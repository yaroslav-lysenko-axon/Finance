using System;
using File.Application.Models.Responses;
using File.Domain.Enums;
using MediatR;

namespace File.Application.Models.Commands
{
    public class GetFileCommand : IRequest<SignedUrlResponse>
    {
        public Guid FileKey { get; set; }
        public ImageSize ImageSize { get; set; }
        public FileType FileType { get; set; }
    }
}
