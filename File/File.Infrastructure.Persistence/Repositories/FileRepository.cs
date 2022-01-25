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
    public class FileRepository : GenericRepository<FileDetails>, IFileRepository
    {
        private readonly FileContext _context;
        public FileRepository(FileContext context)
            : base(context.Files)
        {
            _context = context;
        }

        public async Task<FileDetails> SaveFile(FileDetails fileDetails)
        {
            await _context.Files.AddAsync(fileDetails).ConfigureAwait(false);
            return fileDetails;
        }

        public async Task<List<FileDetails>> SavingMultipleFiles(List<FileDetails> filesDetails)
        {
            await _context.Files.AddRangeAsync(filesDetails).ConfigureAwait(false);
            return filesDetails;
        }

        public async Task<FileDetails> GetFile(Guid imageKey)
        {
            var fileDetails = await _context.Files
                .Include(fileDetail=> fileDetail.ImageDetails)
                .Where(fileDetail => fileDetail.FileKey == imageKey)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return fileDetails;
        }

        public Task DeleteFile(FileDetails fileDetails)
        {
            _context.Files.Remove(fileDetails);
            return Task.CompletedTask;
        }
    }
}
