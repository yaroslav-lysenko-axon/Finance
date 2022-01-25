using System;
using System.Collections.Generic;

namespace File.Domain.Models
{
    public class FileDetails
    {
        public Guid FileKey { get; set; }
        public Guid OwnerId { get; set; }
        public string FileType { get; set; }
        public string OriginalName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RemovedAt { get; set; }
        public ICollection<ImageDetails> ImageDetails { get; set; }
    }
}
