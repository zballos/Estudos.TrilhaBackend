using System;

namespace FileToS3Storage.Api.Models
{
    public class FileS3
    {
        public int Id { get; private set; }
        public string BucketName { get; private set; }
        public string OriginalFileName { get; private set; }
        public string Filename { get; private set; }
        public string ContentType { get; private set; }
        public string FilePath { get; private set; }
        public Guid Identifier { get; private set; }

        protected FileS3() { }
        
        public FileS3(string bucketName, string originalFileName, string filename, string contentType, string filePath, Guid identifier)
        {
            BucketName = bucketName;
            OriginalFileName = originalFileName;
            Filename = filename;
            ContentType = contentType;
            FilePath = filePath;
            Identifier = identifier;
        }
    }
}