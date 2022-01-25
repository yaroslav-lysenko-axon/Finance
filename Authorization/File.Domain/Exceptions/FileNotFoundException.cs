using System.Net;
using File.Domain.Models;

namespace File.Domain.Exceptions
{
    public class FileNotFoundException : AwsFileException
    {
        private const string MessageTemplate = "There is no such file with this identifier:  ";

        public FileNotFoundException(string fileId)
            : base(ErrorCode.FileNotFound, HttpStatusCode.NotFound, MessageTemplate + fileId)
        {
        }
    }
}
