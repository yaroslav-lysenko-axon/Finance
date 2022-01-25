using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using File.Application.Models.Commands;
using File.Domain.ConfigurationClasses;
using File.Domain.Exceptions;
using File.Domain.Repositories;
using File.Domain.Services.Abstraction;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace File.Application.Handlers
{
    public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, OkResult>
    {
        private readonly IFileService _fileService;
        private readonly IImageDetailsService _imageDetailsService;
        private readonly IAmazonS3 _client;
        private readonly IAmazonWebServicesS3Configuration _amazonWebServicesS3Configuration;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFileHandler(
            IFileService fileService,
            IImageDetailsService imageDetailsService,
            IAmazonS3 client,
            IAmazonWebServicesS3Configuration amazonWebServicesS3Configuration,
            IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _imageDetailsService = imageDetailsService;
            _client = client;
            _amazonWebServicesS3Configuration = amazonWebServicesS3Configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var imageDetailsList = await _imageDetailsService.GetImageDetails(request.FileKey);
            var fileDetail = await _fileService.GetFile(request.FileKey);
            if (imageDetailsList.Count == 0 || fileDetail == null)
            {
                throw new FileNotFoundException(request.FileKey.ToString());
            }

            // Delete resized file records from ImageDetails table
            await _imageDetailsService.RemoveImageDetailsList(imageDetailsList).ConfigureAwait(false);

            // Delete original file from the File table
            await _fileService.DeleteFile(fileDetail).ConfigureAwait(false);

            await _unitOfWork.Commit();

            // Delete original file from AWS
            await RemoveFile(request.FileKey, cancellationToken);

            // Delete resized files from AWS
            foreach (var imageDetails in fileDetail.ImageDetails)
            {
                await RemoveFile(imageDetails.ImageKey, cancellationToken);
            }

            return new OkResult();
        }

        private async Task RemoveFile(Guid fileKey, CancellationToken cancellationToken)
        {
            try
            {
                var fileToDelete = await _client.GetObjectAsync(
                    _amazonWebServicesS3Configuration.BucketName,
                    fileKey.ToString(),
                    cancellationToken).ConfigureAwait(false);
                if (fileToDelete == null || fileToDelete.Key.IsNullOrEmpty())
                {
                    throw new FileNotFoundException(fileKey.ToString());
                }

                var deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = _amazonWebServicesS3Configuration.BucketName,
                    Key = fileToDelete.Key,
                };
                await _client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw new Exception("Error occurred: " + amazonS3Exception.Message);
            }
        }
    }
}
