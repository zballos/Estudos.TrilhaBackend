using System;

namespace FileToS3Storage.Api.Models
{
    public class FileS3
    {
        public int Id { get; private set; }
        public string BucketName { get; private set; }
        public string OriginalFileName { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public string FilePath { get; private set; }
        public Guid Identifier { get; private set; }

        protected FileS3() { }
        
        public FileS3(string bucketName, string originalFileName, string fileName, string contentType, string filePath, Guid identifier)
        {
            BucketName = bucketName;
            OriginalFileName = originalFileName;
            FileName = fileName;
            ContentType = contentType;
            FilePath = filePath;
            Identifier = identifier;
        }

        public void SetBucketName(string bucketName)
        {
            BucketName = bucketName;
        }

        public void SetOriginalFileName(string originalFileName)
        {
            OriginalFileName = originalFileName;
        }

        public void SetFileName(string fileName)
        {
            FileName = fileName;
        }

        public void SetContentType(string contentType)
        {
            ContentType = contentType;
        }

        public void SetFilePath(string filePath)
        {
            FilePath = filePath;
        }

        public void SetIdentifier(Guid identifier)
        {
            Identifier = identifier;
        }
    }
}