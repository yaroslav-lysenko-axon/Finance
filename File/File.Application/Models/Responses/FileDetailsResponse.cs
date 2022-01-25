using System;
using System.Collections.Generic;

namespace File.Application.Models.Responses
{
    public class FileDetailsResponse
    {
        public Guid FileKey { get; set; }
        public Guid OwnerId { get; set; }
        public string FileType { get; set; }
        public string OriginalName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RemovedAt { get; set; }
        public ICollection<ImageDetailsResponse> ImageDetailsResponses { get; set; }
    }
}
