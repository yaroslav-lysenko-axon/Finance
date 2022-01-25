using System;

namespace File.Domain.Models
{
    public class ImageDetails
    {
        public Guid ImageKey { get; set; }
        public string Size { get; set; }
        public FileDetails FileDetails { get; set; }
    }
}
