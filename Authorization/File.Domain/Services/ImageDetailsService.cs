using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File.Domain.Enums;
using File.Domain.Models;
using File.Domain.Repositories;
using File.Domain.Services.Abstraction;

namespace File.Domain.Services
{
    public class ImageDetailsService : IImageDetailsService
    {
        private readonly IImageDetailsRepository _imageDetailsRepository;

        public ImageDetailsService(
            IImageDetailsRepository imageDetailsRepository)
        {
            _imageDetailsRepository = imageDetailsRepository;
        }

        public async Task<ImageDetails> UploadImageDetails(FileDetails fileDetails, ImageSize imageSize)
        {
            var imageDetails = new ImageDetails
            {
                ImageKey = Guid.NewGuid(),
                Size = imageSize.ToString(),
                FileDetails = fileDetails,
            };
            return await _imageDetailsRepository.UploadImageDetails(imageDetails);
        }

        public async Task<Guid> GetImageDetailsBySize(Guid fileKey, string imageSize)
        {
            return await _imageDetailsRepository.GetImageDetailsBySize(fileKey, imageSize);
        }

        public async Task<List<ImageDetails>> GetImageDetails(Guid fileKey)
        {
            return await _imageDetailsRepository.GetImageDetails(fileKey);
        }

        public async Task RemoveImageDetailsList(IEnumerable<ImageDetails> imageDetailsList)
        {
            await _imageDetailsRepository.RemoveImageDetailsList(imageDetailsList);
        }
    }
}
