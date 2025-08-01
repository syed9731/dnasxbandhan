using System.Net;

namespace DNAS.Domian.Common
{
    public class CommonResponse<T> where T : new()
    {
        public T Data { get; set; } = new T();
        public ResponseStatus ResponseStatus { get; set; } = new ResponseStatus();
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }

    }
    public class ResponseStatus
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
        public string ResponseStage { get; set; } = string.Empty;
    }
}
