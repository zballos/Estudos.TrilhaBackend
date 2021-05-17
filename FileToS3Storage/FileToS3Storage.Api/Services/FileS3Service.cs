using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Models;
using FileToS3Storage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileToS3Storage.Api.Services
{
    public class FileS3Service : IFileS3Service
    {
        private readonly IAwsS3Service _awsS3Service;
        private readonly IFileS3Repository _fileS3Repository;

        public FileS3Service(IAwsS3Service awsS3Service, IFileS3Repository fileS3Repository)
        {
            _awsS3Service = awsS3Service;
            _fileS3Repository = fileS3Repository;
        }

        public async Task<BaseResponse<DeleteObjectResponse>> DeleteByIdFromS3(int id)
        {
            var fileS3 = _fileS3Repository.GetById(id);

            var response = await _awsS3Service.DeleteFile(fileS3.FilePath);

            return new BaseResponse<DeleteObjectResponse>
            {
                Data = response,
                StatusCode = response.HttpStatusCode
            };
        }

        public async Task<FileStreamResult> DownloadFromS3ById(int id)
        {
            var fileS3 = _fileS3Repository.GetById(id);

            var fileResponse = await _awsS3Service.GetFile(fileS3.FilePath);
            using Stream responseStream = fileResponse.ResponseStream;
            var stream = new MemoryStream();
            await responseStream.CopyToAsync(stream);
            stream.Position = 0;

            return new FileStreamResult(stream, fileResponse.Headers["Content-type"])
            {
                FileDownloadName = fileS3.FileName
            };
        }

        public IList<FileS3> GetAllFromDb()
        {
            return _fileS3Repository.GetAll();
        }

        public FileS3 GetByIdFromDb(int id)
        {
            return _fileS3Repository.GetById(id);
        }

        public async Task<BaseResponse<FileS3>> SaveToS3(IFormFile formFile)
        {
            var s3Response = await _awsS3Service.PutFile(formFile, formFile.FileName);

            FileS3 fileS3 = null;
            if (s3Response.StatusCode == System.Net.HttpStatusCode.OK)
                fileS3 = _fileS3Repository.Add(
                    new FileS3(s3Response.BucketName, formFile.FileName, s3Response.FileName, s3Response.ContentType, s3Response.Key, Guid.Empty));
            
            return new BaseResponse<FileS3>
            {
                StatusCode = s3Response.StatusCode,
                Message = s3Response.Message,
                Data = fileS3
            };
        }
    }
}