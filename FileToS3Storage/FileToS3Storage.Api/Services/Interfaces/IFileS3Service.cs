using System.Threading.Tasks;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Models;
using Microsoft.AspNetCore.Http;

namespace FileToS3Storage.Api.Services.Interfaces
{
    public interface IFileS3Service
    {
        Task<BaseResponse<FileS3>> SaveToS3(IFormFile formFile);
    }
}