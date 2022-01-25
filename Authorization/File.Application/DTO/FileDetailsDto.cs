using System;
using System.Collections.Generic;

namespace File.Application.DTO
{
    public class FileDetailsDto
    {
        public Guid FileKey { get; set; }
        public Guid OwnerId { get; set; }
        public string FileType { get; set; }
        public string OriginalName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RemovedAt { get; set; }
        public List<ImageDetailsDto> ImageDetailsDto { get; set; }
    }
}
