using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FileToS3Storage.Api;
using FileToS3Storage.Api.Services;
using Moq;
using Xunit;

namespace FileToS3Storage.Test
{
    public class AwsS3ServiceTests
    {
        private AppSettingsConfig _appSettings;
        private readonly Mock<IAmazonS3> _amazonS3Mock;
        private readonly AwsS3Service _awsS3Service;

        public AwsS3ServiceTests()
        {
            _appSettings = new AppSettingsConfig();
            _appSettings.AwsS3Bucket = "s3-teste";
            _amazonS3Mock = new Mock<IAmazonS3>();

            _awsS3Service = new AwsS3Service(_amazonS3Mock.Object, _appSettings);
        }

        [Fact]
        public async void DeveDeletarArquivoPeloNome()
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
    }
}