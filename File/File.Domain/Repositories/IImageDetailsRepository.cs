using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File.Domain.Models;

namespace File.Domain.Repositories
{
    public interface IImageDetailsRepository
    {
        Task<ImageDetails> UploadImageDetails(ImageDetails imageDetails);
        Task<Guid> GetImageDetailsBySize(Guid fileKey, string imageSize);
        Task<List<ImageDetails>> GetImageDetails(Guid fileKey);
        Task RemoveImageDetailsList(IEnumerable<ImageDetails> imageDetailsList);
    }
}
