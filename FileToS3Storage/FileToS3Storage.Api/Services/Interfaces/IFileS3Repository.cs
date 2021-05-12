using FileToS3Storage.Api.Models;

namespace FileToS3Storage.Api.Services.Interfaces
{
    public interface IFileS3Repository : IRepository<int, FileS3>
    {
         
    }
}