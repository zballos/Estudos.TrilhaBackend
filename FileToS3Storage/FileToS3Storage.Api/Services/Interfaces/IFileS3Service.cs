using System.Collections.Generic;
using System.Threading.Tasks;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileToS3Storage.Api.Services.Interfaces
{
    public interface IFileS3Service
    {
        Task<BaseResponse<FileS3>> SaveToS3(IFormFile formFile);
        Task<FileStreamResult> DownloadFromS3ById(int id);
        Task<BaseResponse<bool>> DeleteByIdFromS3(int id);
        IList<FileS3> GetAllFromDb();
        FileS3 GetByIdFromDb(int id);
    }
}