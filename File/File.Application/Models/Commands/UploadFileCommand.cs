using System;
using File.Application.DTO;
using File.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace File.Application.Models.Commands
{
    public class UploadFileCommand : IRequest<FileDetailsDto>
    {
        public Guid OwnerId { get; set; }
        public FileType FileType { get; set; }
        public IFormFile File { get; set; }
    }
}
