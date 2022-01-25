using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File.Domain.Enums;
using File.Domain.Models;

namespace File.Domain.Services.Abstraction
{
    public interface IImageDetailsService
    {
        Task<ImageDetails> UploadImageDetails(FileDetails fileDetails, ImageSize imageSize);
        Task<Guid> GetImageDetailsBySize(Guid fileKey, string imageSize);
        Task<List<ImageDetails>> GetImageDetails(Guid fileKey);
        Task RemoveImageDetailsList(IEnumerable<ImageDetails> imageDetailsList);
    }
}
