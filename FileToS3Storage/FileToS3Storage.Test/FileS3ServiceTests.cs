using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Bogus;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Models;
using FileToS3Storage.Api.Services;
using FileToS3Storage.Api.Services.Interfaces;
using FileToS3Storage.Test.Builders;
using Moq;
using Xunit;

namespace FileToS3Storage.Test
{
    public class FileS3ServiceTests
    {
        private readonly Faker _faker;
        private readonly Mock<IAwsS3Service> _awsS3ServiceMock;
        private readonly Mock<IFileS3Repository> _fileS3RepositoryMock;
        private readonly FileS3Service _fileS3Service;

        public FileS3ServiceTests()
        {
            _faker = new Faker();
            _awsS3ServiceMock = new Mock<IAwsS3Service>();
            _fileS3RepositoryMock = new Mock<IFileS3Repository>();

            _fileS3Service = new FileS3Service(_awsS3ServiceMock.Object, _fileS3RepositoryMock.Object);
        }

        private FileS3 BuildFileS3()
        {
            var fileExtension = _faker.PickRandomParam(SomeFileExtensions.Examples());
            string filename = _faker.Random.AlphaNumeric(10) + fileExtension.Key;
            string myBucket = "my-bucket-s3";
            return new FileS3(myBucket, filename, filename, fileExtension.Value, filename, Guid.Empty);
        } 

        [Fact]
        public async Task DeveDeletarPorIdDoS3Storage()
        {
            int id = 1;
            var fileS3 = BuildFileS3();
            var response = new DeleteObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };
            
            _fileS3RepositoryMock.Setup(repo => repo.GetById(id)).Returns(fileS3);
            _awsS3ServiceMock.Setup(aws => aws.DeleteFile(fileS3.FileName)).ReturnsAsync(response);

            var result = await _fileS3Service.DeleteByIdFromS3(id);

            Assert.IsType<BaseResponse<DeleteObjectResponse>>(result);
            Assert.NotNull(result.Data);
            Assert.True(result.StatusCode > 0);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}