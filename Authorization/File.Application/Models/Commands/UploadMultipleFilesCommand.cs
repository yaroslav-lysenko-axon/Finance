using System;
using System.Collections.Generic;
using File.Application.DTO;
using File.Application.Models.Responses;
using File.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace File.Application.Models.Commands
{
    public class UploadMultipleFilesCommand : IRequest<List<FileDetailsDto>>
    {
        public Guid OwnerId { get; set; }
        public FileType FileType { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
