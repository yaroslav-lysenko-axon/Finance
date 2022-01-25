using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Domain.Enums;
using File.Domain.Models;
using File.Domain.Repositories;
using File.Domain.Services.Abstraction;

namespace File.Domain.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ITimeProvider _timeProvider;
        public FileService(
            IFileRepository fileRepository,
            ITimeProvider timeProvider)
        {
            _fileRepository = fileRepository;
            _timeProvider = timeProvider;
        }

        public async Task<FileDetails> SaveFile(Guid ownerId, FileType fileType, string fileName)
        {
            var fileDetails = new FileDetails
            {
                FileKey = Guid.NewGuid(),
                OwnerId = ownerId,
                FileType = fileType.ToString(),
                OriginalName = fileName,
                CreatedAt = _timeProvider.UtcNow(),
            };
            return await _fileRepository.SaveFile(fileDetails);
        }

        public async Task<List<FileDetails>> SavingMultipleFiles(Guid ownerId, FileType fileType, IEnumerable<string> fileNamesList)
        {
            var fileDetailsList = fileNamesList.Select(fileName => new FileDetails
                {
                    FileKey = Guid.NewGuid(),
                    OwnerId = ownerId,
                    FileType = fileType.ToString(),
                    OriginalName = fileName,
                    CreatedAt = _timeProvider.UtcNow(),
                })
                .ToList();

            return await _fileRepository.SavingMultipleFiles(fileDetailsList);
        }

        public async Task<FileDetails> GetFile(Guid imageKey)
        {
            return await _fileRepository.GetFile(imageKey);
        }

        public async Task DeleteFile(FileDetails fileDetails)
        {
            await _fileRepository.DeleteFile(fileDetails);
        }
    }
}
