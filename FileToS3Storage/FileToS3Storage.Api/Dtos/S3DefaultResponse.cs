using System.Net;

namespace FileToS3Storage.Api.Dtos
{
    public class S3DefaultResponse
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string FileName { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string ContentType { get; set; }
    }
}