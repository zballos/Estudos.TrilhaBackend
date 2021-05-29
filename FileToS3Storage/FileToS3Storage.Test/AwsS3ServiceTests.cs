using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Bogus;
using FileToS3Storage.Api;
using FileToS3Storage.Api.Dtos;
using FileToS3Storage.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Moq;
using Xunit;

namespace FileToS3Storage.Test
{
    public class AwsS3ServiceTests
    {
        private AppSettingsConfig _appSettings;
        private readonly Mock<IAmazonS3> _amazonS3Mock;
        private readonly AwsS3Service _awsS3Service;
        private readonly Faker _faker;

        public AwsS3ServiceTests()
        {
            _faker = new Faker();
            _appSettings = new AppSettingsConfig();
            _appSettings.AwsS3Bucket = "s3-teste";
            _amazonS3Mock = new Mock<IAmazonS3>();

            _awsS3Service = new AwsS3Service(_amazonS3Mock.Object, _appSettings);
        }

        [Fact]
        public async Task DeveDeletarArquivoPeloNome()
        {
            string fileName = "teste-imagem.jpeg";
            var response = new DeleteObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };
            _amazonS3Mock
                .Setup(s3 => s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
                .ReturnsAsync(response);

            var result = await _awsS3Service.DeleteFile(fileName);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("path-to-folder")]
        [InlineData(" path-to-folder")]
        public async Task DeveEnviarArquivoParaS3Storage(string path)
        {
            var file = GetMockedFormFile();

            var putResponse = new PutObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK
            };
            _amazonS3Mock
                .Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(putResponse);

            var result = await _awsS3Service.PutFile(file, file.FileName, path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEmpty(result.BucketName);
            Assert.NotEmpty(result.ContentType);
            Assert.NotEmpty(result.FileName);
            Assert.NotEmpty(result.Key);
            Assert.IsType<S3DefaultResponse>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("path-to-folder")]
        [InlineData(" path-to-folder")]
        public async Task NaoDeveEnviarArquivoInvalidoParaS3Storage(string path)
        {
            var file = GetMockedFormFile();
            _amazonS3Mock
                .Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(It.IsAny<PutObjectResponse>());

            var result = await _awsS3Service.PutFile(file, file.FileName, path);

            Assert.NotNull(result);
            Assert.NotEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.NotEmpty(result.Message);
            Assert.IsType<S3DefaultResponse>(result);
        }

        [Fact]
        public async Task DeveObterArquivoDoS3Storage()
        {
            var file = GetMockedFormFile();
            var getResponse = new GetObjectResponse
            {
                BucketName = _appSettings.AwsS3Bucket,
                ResponseStream = file.OpenReadStream(),
                ContentLength = file.Length,
                Key = file.FileName,
                HttpStatusCode = HttpStatusCode.OK
            };

            _amazonS3Mock
                .Setup(s3 => s3.GetObjectAsync(It.IsAny<GetObjectRequest>(), default))
                .ReturnsAsync(getResponse);

            var result = await _awsS3Service.GetFile(file.FileName);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.NotEmpty(result.BucketName);
            Assert.NotNull(result.ResponseStream);
            Assert.True(result.ContentLength > 0);
        }

        private IFormFile GetMockedFormFile()
        {
            string fileName = $"{_faker.Random.AlphaNumeric(15)}.txt";
            var fileMock = new Mock<IFormFile>();
            var content = _faker.Random.Words(100);
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);

            return fileMock.Object;
        }
    }
}