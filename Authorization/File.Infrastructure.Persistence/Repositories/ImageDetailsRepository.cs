using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Domain.Models;
using File.Domain.Repositories;
using File.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace File.Infrastructure.Persistence.Repositories
{
    public class ImageDetailsRepository : GenericRepository<ImageDetails>, IImageDetailsRepository
    {
        private readonly FileContext _context;
        public ImageDetailsRepository(FileContext context)
            : base(context.Images)
        {
            _context = context;
        }

        public async Task<ImageDetails> UploadImageDetails(ImageDetails imageDetails)
        {
            await _context.Images.AddAsync(imageDetails).ConfigureAwait(false);
            return imageDetails;
        }

        public async Task<Guid> GetImageDetailsBySize(Guid fileKey, string imageSize)
        {
            var imageKey = await _context.Images
                .Where(imageDetails => imageDetails.FileDetails.FileKey == fileKey && imageDetails.Size == imageSize)
                .Select(imageDetails=>imageDetails.ImageKey)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return imageKey;
        }

        public async Task<List<ImageDetails>> GetImageDetails(Guid fileKey)
        {
            var imageKey = await _context.Images
                .Where(imageDetails => imageDetails.FileDetails.FileKey == fileKey)
                .ToListAsync().ConfigureAwait(false);
            return imageKey;
        }

        public Task RemoveImageDetailsList(IEnumerable<ImageDetails> imageDetailsList)
        {
            _context.Images.RemoveRange(imageDetailsList);
            return Task.CompletedTask;
        }
    }
}
