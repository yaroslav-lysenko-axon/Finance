using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Authorization.Application.Extensions;
using AutoMapper;
using File.Application.DTO;
using File.Application.Extensions;
using File.Application.Models.Commands;
using File.Domain.ConfigurationClasses;
using File.Domain.Enums;
using File.Domain.Exceptions;
using File.Domain.Models;
using File.Domain.Repositories;
using File.Domain.Services.Abstraction;
using MediatR;
using SixLabors.ImageSharp;

namespace File.Application.Handlers
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, FileDetailsDto>
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
        private readonly IAmazonWebServicesS3Configuration _amazonWebServicesS3Configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UploadFileHandler(
            IFileService fileService,
            IImageDetailsService imageDetailsService,
            IAmazonS3 client,
            IAmazonWebServicesS3Configuration minioConfiguration,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _fileService = fileService;
            _imageDetailsService = imageDetailsService;
            _client = client;
            _amazonWebServicesS3Configuration = minioConfiguration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FileDetailsDto> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            // upload original file to File table
            var uploadFileDetails = await _fileService.SaveFile(request.OwnerId, request.FileType, request.File.FileName);
            var originalFileKey = uploadFileDetails.FileKey.ToString();

            // upload original file to AWS
            var originalFileBytes = await request.File.GetBytes();
            await UploadFile(request, originalFileBytes, null, originalFileKey, cancellationToken).ConfigureAwait(false);
            try
            {
                if (request.FileType == FileType.Image)
                {
                    using var image = await Image.LoadAsync(request.File.OpenReadStream());
                    foreach (var uploadSize in ImageSizes)
                    {
                        // upload resized files to ImageDetails table
                        var imageDetails = await _imageDetailsService.UploadImageDetails(uploadFileDetails, uploadSize);
                        uploadFileDetails.ImageDetails.Add(imageDetails);

                        var fileBytes = await ImagesExtensions.GetFileBytesAfterResizing(image, (int)uploadSize, cancellationToken);

                        // upload resized files to AWS
                        await UploadFile(request, fileBytes, imageDetails, null, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (AmazonS3Exception)
            {
                throw new AwsServiceInternalException();
            }

            await _unitOfWork.Commit();

            var response = _mapper.Map<FileDetailsDto>(uploadFileDetails);
            return response;
        }

        private async Task UploadFile(UploadFileCommand request, byte[] fileBytes, ImageDetails imageDetails, string originalFileKey, CancellationToken cancellationToken)
        {
            var originalFile = true;
            var fileNameGuid = imageDetails?.ImageKey;
            if (fileNameGuid.HasValue)
            {
                originalFile = false;
            }

            await using var stream = new MemoryStream(fileBytes);
            var beforeUpload = stream.Position;
            try
            {
                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = _amazonWebServicesS3Configuration.BucketName,
                    ContentType = request.File.ContentType,
                    Key = originalFile ? originalFileKey : fileNameGuid.ToString(),
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
