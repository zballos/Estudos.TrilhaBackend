using System.Net;

namespace FileToS3Storage.Api.Dtos
{
    public class BaseResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public BaseResponse(HttpStatusCode statusCode, string message, T data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }
}