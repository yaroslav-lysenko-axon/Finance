using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using File.Application.Models.Commands;
using File.Application.Models.Responses;
using File.Domain.ConfigurationClasses;
using File.Domain.Enums;
using File.Domain.Exceptions;
using File.Domain.Services.Abstraction;
using MediatR;

namespace File.Application.Handlers
{
public class GetSignedUrlHandler : IRequestHandler<GetFileCommand, SignedUrlResponse>
    {
        private readonly IImageDetailsService _imageDetailsService;
        private readonly IAmazonS3 _client;
        private readonly IAmazonWebServicesS3Configuration _amazonWebServicesS3Configuration;
        public GetSignedUrlHandler(
            IImageDetailsService imageDetailsService,
            IAmazonS3 client,
            IAmazonWebServicesS3Configuration amazonWebServicesS3Configuration)
        {
            _imageDetailsService = imageDetailsService;
            _client = client;
            _amazonWebServicesS3Configuration = amazonWebServicesS3Configuration;
        }

        public async Task<SignedUrlResponse> Handle(GetFileCommand request, CancellationToken cancellationToken)
        {
            var requestHasSize = request.ImageSize != 0;
            var fileKey = Guid.Empty;
            if (request.FileType == FileType.Image)
            {
                fileKey = requestHasSize
                    ? await _imageDetailsService.GetImageDetailsBySize(request.FileKey, request.ImageSize.ToString())
                    : request.FileKey;
            }

            try
            {
                var myBucketObjects = await _client.ListObjectsAsync(_amazonWebServicesS3Configuration.BucketName, cancellationToken).ConfigureAwait(false);
                var gettingFile = myBucketObjects.S3Objects.FirstOrDefault(s3Object => s3Object.Key.StartsWith(fileKey.ToString(), StringComparison.CurrentCulture));
                if (gettingFile == null || !gettingFile.Key.Any())
                {
                    throw new FileNotFoundException(fileKey.ToString());
                }

                var expiryUrlRequest = new Amazon.S3.Model.GetPreSignedUrlRequest
                {
                    BucketName = _amazonWebServicesS3Configuration.BucketName,
                    Key = gettingFile.Key,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Verb = HttpVerb.GET,
                    Protocol = Protocol.HTTP,
                };

                var response = new SignedUrlResponse
                {
                    SignedUrl = _client.GetPreSignedURL(expiryUrlRequest),
                };
                return response;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw new Exception("Error occurred: " + amazonS3Exception.Message);
            }
        }
    }
}
