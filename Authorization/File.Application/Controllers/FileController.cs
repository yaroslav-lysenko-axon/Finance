using System;
using System.Threading.Tasks;
using File.Application.Extensions;
using File.Application.Models.Commands;
using File.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace File.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Upload file.
        /// </summary>
        /// <remarks>
        /// Uploading a file to Amazon Simple Storage Service.
        /// </remarks>
        /// <param name="fileType">File type. </param>
        /// <param name="file">File to uploading.</param>
        /// <response code="200">The upload has been OK.</response>
        [ProducesResponseType(200)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
            [FromQuery] FileType fileType,
            [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Content("No file selected for upload.");
            }

            var ownerId = _httpContextAccessor.HttpContext?.User.GetAuthorizedUserId();
            var request = new UploadFileCommand
            {
                OwnerId = ownerId.GetValueOrDefault(),
                FileType = fileType,
                File = file,
            };

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Get url of file.
        /// </summary>
        /// <remarks>
        /// Get file url from Amazon Simple Storage Service.
        /// </remarks>
        /// <param name="fileKey">Id of the File to receive.</param>
        /// <param name="fileType">Type of the File to receive.</param>
        /// <param name="imageSize">Size of the File to receive.</param>
        /// <response code="200">The operation has been OK.</response>
        [ProducesResponseType(200)]
        [HttpGet("{fileKey:guid}")]
        public async Task<IActionResult> GetSignedUrl(Guid fileKey, [FromQuery] FileType fileType, ImageSize imageSize)
        {
            var request = new GetFileCommand
            {
                FileKey = fileKey,
                FileType = fileType,
                ImageSize = imageSize,
            };

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Removed file.
        /// </summary>
        /// <remarks>
        /// Removing a file from Amazon Simple Storage Service.
        /// </remarks>
        /// <response code="204">Deleted. No Content.</response>
        [ProducesResponseType(204)]
        [HttpDelete("remove")]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteFileCommand request)
        {
            await _mediator.Send(request);
            return StatusCode(204);
        }

        /// <summary>
        /// Upload files.
        /// </summary>
        /// <remarks>
        /// Uploading a files to Amazon Simple Storage Service.
        /// </remarks>
        /// <param name="fileType">File type. </param>
        /// <param name="files">Files to uploading.</param>
        /// <response code="200">The upload has been OK.</response>
        [ProducesResponseType(200)]
        [HttpPost("multiple")]
        public async Task<IActionResult> UploadMultipleFile(
            [FromQuery] FileType fileType,
            [FromForm] IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
            {
                return Content("No file selected for upload.");
            }

            var ownerId = _httpContextAccessor.HttpContext?.User.GetAuthorizedUserId();
            var request = new UploadMultipleFilesCommand
            {
                OwnerId = ownerId.GetValueOrDefault(),
                FileType = fileType,
                Files = files,
            };

            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
