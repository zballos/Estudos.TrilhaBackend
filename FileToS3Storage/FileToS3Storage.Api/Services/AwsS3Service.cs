using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FileToS3Storage.Api.Services
{
    public class AwsS3Service : IAwsS3Service
    {
        private const string SEPARATOR = "/";
        private readonly IAmazonS3 _amazonS3;
        private readonly AppSettingsConfig _appSettings;

        public AwsS3Service(IAmazonS3 amazonS3, AppSettingsConfig appSettings)
        {
            _amazonS3 = amazonS3;
            _appSettings = appSettings;
        }

        public async Task<DeleteObjectResponse> DeleteFile(string filename)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _appSettings.AwsS3Bucket,
                Key = filename
            };

            var response = await _amazonS3.DeleteObjectAsync(request);

            return response;
        }

        public async Task<GetObjectResponse> GetFile(string filename)
        {
            var request = new GetObjectRequest
            {
                BucketName = _appSettings.AwsS3Bucket,
                Key = filename
            };

            GetObjectResponse response = await _amazonS3.GetObjectAsync(request);

            return response;
        }

        public async Task<S3DefaultResponse> PutFile(IFormFile formFile, string filename, string filePath = null)
        {
            try {
                var request = new PutObjectRequest()
                {
                    BucketName = _appSettings.AwsS3Bucket,
                    Key = JoinFolderAndFilename(filePath, filename),
                    InputStream = formFile.OpenReadStream(),
                    ContentType = formFile.ContentType
                };
                
                var result = await _amazonS3.PutObjectAsync(request);

                return new S3DefaultResponse 
                {
                    BucketName = request.BucketName,
                    Key = request.Key,
                    FileName = filename,
                    ContentType = request.ContentType,
                    StatusCode = result.HttpStatusCode
                };
            } catch (Exception ex) {
                return InfoBadRequest(ex.Message);
            }
        }

        private string JoinFolderAndFilename(string folder, string filename)
        {
            if (!string.IsNullOrEmpty(folder?.Trim())) return string.Join(SEPARATOR, folder, filename);
            
            return filename;
        }

        private S3DefaultResponse InfoBadRequest(string message)
        {
            return new S3DefaultResponse
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = message
            };
        }
    }
}