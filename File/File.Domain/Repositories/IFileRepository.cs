using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File.Domain.Models;

namespace File.Domain.Repositories
{
    public interface IFileRepository : IGenericRepository<FileDetails>
    {
        Task<FileDetails> SaveFile(FileDetails imageDetails);
        Task<List<FileDetails>> SavingMultipleFiles(List<FileDetails> filesDetails);
        Task<FileDetails> GetFile(Guid fileKey);
        Task DeleteFile(FileDetails fileDetails);
    }
}
