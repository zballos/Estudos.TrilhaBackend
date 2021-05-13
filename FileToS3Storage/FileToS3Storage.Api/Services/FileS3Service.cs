using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public Task<BaseResponse<bool>> DeleteByIdFromS3(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<FileStreamResult> DownloadFromS3ById(int id)
        {
            var fileS3 = _fileS3Repository.GetById(id);

            if (fileS3 == null)
                return null;

            var fileResponse = await _awsS3Service.GetFile(fileS3.FilePath);
            using Stream responseStream = fileResponse.ResponseStream;
            var stream = new MemoryStream();
            await responseStream.CopyToAsync(stream);
            stream.Position = 0;

            return new FileStreamResult(stream, fileResponse.Headers["Content-type"])
            {
                FileDownloadName = fileS3.Filename
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
            
            return new BaseResponse<FileS3>(
                s3Response.StatusCode,
                s3Response.Message,
                fileS3
            );
        }
    }
}