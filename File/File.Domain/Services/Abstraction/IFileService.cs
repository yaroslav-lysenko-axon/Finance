using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using File.Domain.Enums;
using File.Domain.Models;

namespace File.Domain.Services.Abstraction
{
    public interface IFileService
    {
        Task<FileDetails> SaveFile(Guid ownerId, FileType fileType, string fileName);
        Task<List<FileDetails>> SavingMultipleFiles(Guid ownerId, FileType fileType, IEnumerable<string> fileName);
        Task<FileDetails> GetFile(Guid imageKey);
        Task DeleteFile(FileDetails fileDetails);
    }
}
