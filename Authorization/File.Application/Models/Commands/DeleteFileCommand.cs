using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace File.Application.Models.Commands
{
    public class DeleteFileCommand : IRequest<OkResult>
    {
        public Guid FileKey { get; set; }
    }
}
