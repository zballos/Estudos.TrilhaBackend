using FileToS3Storage.Models;

namespace FileToS3Storage.Services.Interfaces
{
    public interface IFileS3Repository : IRepository<int, FileS3>
    {
         
    }
}