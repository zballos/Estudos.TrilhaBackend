using System.Collections.Generic;
namespace FileToS3Storage.Test.Builders
{
    public class SomeFileExtensions
    {
        public static KeyValuePair<string, string>[] Examples()
        {
            return new[] {
                KeyValuePair.Create(".jpg", "image/jpeg"),
                KeyValuePair.Create(".jpeg", "image/jpeg"),
                KeyValuePair.Create(".pdf", "application/pdf"),
                KeyValuePair.Create(".csv", "text/csv"),
                KeyValuePair.Create(".json", "application/json"),
                KeyValuePair.Create(".xml", "application/xml")
            };
        }
    }
}