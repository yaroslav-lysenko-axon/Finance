using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using AutoMapper;
using File.Application.DTO;
using File.Application.Extensions;
using File.Application.Models.Commands;
using File.Application.Models.Responses;
using File.Domain.ConfigurationClasses;
using File.Domain.Enums;
using File.Domain.Exceptions;
using File.Domain.Models;
using File.Domain.Repositories;
using File.Domain.Services.Abstraction;
using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace File.Application.Handlers
{
 public class UploadMultipleFilesHandler : IRequestHandler<UploadMultipleFilesCommand, List<FileDetailsDto>>
    {
        private static readonly IReadOnlyCollection<ImageSize> ImageSizes = new[]
        {
            ImageSize.Thumbnail,
            ImageSize.Small,
            ImageSize.Medium,
            ImageSize.Large,
        };

        private readonly IFileService _fileService;
        private readonly IImageDetailsService _imageDetailsService;
        private readonly IAmazonS3 _client;
        private readonly IMapper _mapper;
        private readonly IAmazonWebServicesS3Configuration _amazonWebServicesS3Configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UploadMultipleFilesHandler(
            IFileService fileService,
            IImageDetailsService imageDetailsService,
            IAmazonS3 client,
            IMapper mapper,
            IAmazonWebServicesS3Configuration amazonWebServicesS3Configuration,
            IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _imageDetailsService = imageDetailsService;
            _client = client;
            _mapper = mapper;
            _amazonWebServicesS3Configuration = amazonWebServicesS3Configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FileDetailsDto>> Handle(
            UploadMultipleFilesCommand request,
            CancellationToken cancellationToken)
        {
            var originalFileBytesList = new List<byte[]>();
            foreach (var file in request.Files)
            {
                var fileByte = await file.GetBytes();
                originalFileBytesList.Add(fileByte);
            }

            var fileNames = request.Files.Select(file => file.FileName).ToList();

            // upload original files to File table
            var uploadFileDetailsList =
                await _fileService.SavingMultipleFiles(request.OwnerId, request.FileType, fileNames);
            var originalFileKeyList = uploadFileDetailsList.Select(fileDetails => fileDetails.FileKey).ToList();

            // upload original files to AWS
            await UploadFiles(request, originalFileBytesList, null, originalFileKeyList, cancellationToken)
                .ConfigureAwait(false);

            try
            {
                if (request.FileType == FileType.Image)
                {
                    var addedFilesList = uploadFileDetailsList
                        .Zip(
                            request.Files,
                            (fileDetailsList, files) => new { fileDetails = fileDetailsList, file = files });
                    foreach (var addedFile in addedFilesList)
                    {
                        var fileBytesList = new List<byte[]>();
                        using var image = await Image.LoadAsync(addedFile.file.OpenReadStream());
                        foreach (var uploadSize in ImageSizes)
                        {
                            // upload resized images to ImageDetails table
                            var imageDetails =
                                await _imageDetailsService.UploadImageDetails(addedFile.fileDetails, uploadSize);
                            await using var outStream = new MemoryStream();
                            var size = (int)uploadSize;
                            var (width, height) =
                                ImagesExtensions.CalculateResizedImageDimensions(image.Width, image.Height, size);
                            image.Mutate(operation => operation.Resize(width, height));
                            await image.SaveAsync(outStream, PngFormat.Instance, cancellationToken);
                            await outStream.FlushAsync(cancellationToken);
                            var fileBytes = outStream.ToArray();
                            fileBytesList.Add(fileBytes);
                            addedFile.fileDetails.ImageDetails.Add(imageDetails);

                            // upload resized images to AWS
                            await UploadFiles(request, fileBytesList, imageDetails, originalFileKeyList,
                                cancellationToken).ConfigureAwait(false);
                            fileBytesList.Clear();
                        }
                    }
                }
            }
            catch (AmazonS3Exception)
            {
                throw new AwsServiceInternalException();
            }

            await _unitOfWork.Commit();

            var response = _mapper.Map<List<FileDetailsDto>>(uploadFileDetailsList);
            return response;
        }

        private async Task UploadFiles(UploadMultipleFilesCommand request, IEnumerable<byte[]> fileBytesList,
            ImageDetails imageDetails,
            IEnumerable<Guid> fileKeyList, CancellationToken cancellationToken)
        {
            var addedFilesList = fileKeyList
                .Zip(fileBytesList, (key, bytes) => new { fileKey = key, fileBytes = bytes })
                .Zip(request.Files, (keyAndBytesList, files) => new { keyAndBytes = keyAndBytesList, file = files });
            foreach (var fileInfo in addedFilesList)
            {
                var originalFile = true;
                var fileNameGuid = imageDetails?.ImageKey;
                if (fileNameGuid.HasValue)
                {
                    originalFile = false;
                }

                await using var stream = new MemoryStream(fileInfo.keyAndBytes.fileBytes);
                var beforeUpload = stream.Position;
                try
                {
                    var putRequest = new Amazon.S3.Model.PutObjectRequest
                    {
                        BucketName = _amazonWebServicesS3Configuration.BucketName,
                        ContentType = fileInfo.file.ContentType,
                        Key = originalFile ? fileInfo.keyAndBytes.fileKey.ToString() : fileNameGuid.ToString(),
                        InputStream = stream,
                    };
                    await _client.PutObjectAsync(putRequest, cancellationToken);
                }
                catch
                {
                    stream.Position = beforeUpload;
                    stream.SetLength(stream.Position);
                    throw;
                }
            }
        }
    }
}
