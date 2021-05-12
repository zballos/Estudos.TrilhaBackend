using System;

namespace FileToS3Storage.Models
{
    public class FileS3
    {
        public int Id { get; private set; }
        public string Filename { get; private set; }
        public string MimeType { get; private set; }
        public Guid Identifier { get; private set; }

        protected FileS3() { }
        
        public FileS3(string filename, string mimeType, Guid identifier)
        {
            Filename = filename;
            MimeType = mimeType;
            Identifier = identifier;
        }
    }
}