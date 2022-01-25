using System.ComponentModel.DataAnnotations;

namespace File.Domain.Models
{
    public enum ErrorCode
    {
        [Display(Name = "file_not_found")]
        FileNotFound,
        [Display(Name = "internal_service_error")]
        InternalServiceError,
    }
}
