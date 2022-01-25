using System.Collections.Generic;

namespace Authorization.Application.ApiError
{
    public class ApiError
    {
        public string OperationId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public List<ErrorDetail> Details { get; set; }
    }
}
