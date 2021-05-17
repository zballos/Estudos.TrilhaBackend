using System;
using System.Collections.Generic;
using Bogus;
using FileToS3Storage.Api.Database;
using FileToS3Storage.Api.Models;
using FileToS3Storage.Api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FileToS3Storage.Test
{
    public class FileS3RepositoryTests
    {
        private readonly Faker _faker;
        private const string DatabaseName = "FileStorageDbTest";
        private KeyValuePair<string, string>[] SomeFileExtensions = new[] {
            KeyValuePair.Create(".jpg", "image/jpeg"),
            KeyValuePair.Create(".jpeg", "image/jpeg"),
            KeyValuePair.Create(".pdf", "application/pdf"),
            KeyValuePair.Create(".csv", "text/csv"),
            KeyValuePair.Create(".json", "application/json"),
            KeyValuePair.Create(".xml", "application/xml")
        };

        public FileS3RepositoryTests()
        {
            _faker = new Faker();
        }

        private DbContextOptions<FileDbContext> GetDbContextOptions()
        {
            var options = new DbContextOptionsBuilder<FileDbContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;

            return options;
        }

        private FileS3 GetNewRandomFileS3() 
        {
            var fileExtension = _faker.PickRandomParam(SomeFileExtensions);
            string originalFileName = $"{_faker.Name}{fileExtension.Key}";
            
            return new FileS3(
                "s3-bucket-name",
                originalFileName,
                originalFileName,
                fileExtension.Value,
                originalFileName,
                Guid.Empty
            );
        }

        [Fact]
        public void DeveSalvarComSucesso()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);
                var fileS3 = GetNewRandomFileS3();

                var fileS3Result = fileS3Repository.Add(fileS3);

                Assert.NotNull(fileS3Result);
            }
        }

        [Fact]
        public void DeveRetornarTodosRegistros()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);

                List<FileS3> files = new List<FileS3>();
                files.Add(fileS3Repository.Add(GetNewRandomFileS3()));
                files.Add(fileS3Repository.Add(GetNewRandomFileS3()));
                files.Add(fileS3Repository.Add(GetNewRandomFileS3()));

                var fileS3Result = fileS3Repository.GetAll();

                Assert.NotNull(fileS3Result);
                Assert.Equal(files.Count, fileS3Result.Count);
            }
        }

        [Fact]
        public void DeveRetornarListaVazia()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var fileS3Repository = new FileS3Repository(context);
                var fileS3Result = fileS3Repository.GetAll();

                Assert.NotNull(fileS3Result);
                Assert.Equal(0, fileS3Result.Count);
            }
        }

        [Fact]
        public void DeveRetornarArquivoPorId()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);
                var fileS3 = fileS3Repository.Add(GetNewRandomFileS3());

                var fileS3Result = fileS3Repository.GetById(fileS3.Id);

                Assert.NotNull(fileS3Result);
                Assert.Equal(fileS3.Id, fileS3Result.Id);
            }
        }

        [Fact]
        public void DeveRetornarArquivoNuloPorIdSeNaoExistir()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);
                int invalidId = 0;
                var fileS3Result = fileS3Repository.GetById(invalidId);

                Assert.Null(fileS3Result);
            }
        }

        [Fact]
        public void DeveAtualizarArquivo()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);
                var fileS3 = fileS3Repository.Add(GetNewRandomFileS3());
                string newBucketName = "new-bucket-name";
                string newFileName = "new-filename.png";
                string mimeType = "image/png";
                fileS3.SetBucketName(newBucketName);
                fileS3.SetFileName(newFileName);
                fileS3.SetFilePath(newFileName);
                fileS3.SetContentType(mimeType);

                var fileS3Result = fileS3Repository.Update(fileS3);

                Assert.NotNull(fileS3Result);
                Assert.Equal(newBucketName, fileS3Result.BucketName);
            }
        }

        [Fact]
        public void DeveDeletarArquivo()
        {
            using (var context = new FileDbContext(GetDbContextOptions())) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var fileS3Repository = new FileS3Repository(context);
                var fileS3 = fileS3Repository.Add(GetNewRandomFileS3());

                fileS3Repository.Delete(fileS3);
                var fileS3Result = fileS3Repository.GetById(fileS3.Id);

                Assert.NotNull(fileS3);
                Assert.Null(fileS3Result);
            }
        }
    }
}
