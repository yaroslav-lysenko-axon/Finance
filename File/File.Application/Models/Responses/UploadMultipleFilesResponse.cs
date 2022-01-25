using System;
using System.Collections.Generic;

namespace File.Application.Models.Responses
{
    public class UploadMultipleFilesResponse
    {
        public List<Guid> AvatarIds { get; set; }
    }
}
