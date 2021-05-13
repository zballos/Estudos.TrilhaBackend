using System.Threading.Tasks;
using Amazon.S3.Model;
using FileToS3Storage.Api.Dtos;
using Microsoft.AspNetCore.Http;

namespace FileToS3Storage.Api.Services.Interfaces
{
    public interface IAwsS3Service
    {
        Task<S3DefaultResponse> PutFile(IFormFile formFile, string filename, string filePath = null);
        Task<GetObjectResponse> GetFile(string filename);
        Task<DeleteObjectResponse> DeleteFile(string filename, string filepath = null);
    }
}