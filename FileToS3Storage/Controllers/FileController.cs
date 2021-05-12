using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FileToS3Storage.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileToS3Storage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IFileS3Repository _files3Repository;

        public FileController(IAmazonS3 amazonS3, IFileS3Repository files3Repository)
        {
            _amazonS3 = amazonS3;
            _files3Repository = files3Repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            Guid folderGuid = Guid.NewGuid();

            var request = new PutObjectRequest()
            {
                BucketName = "s3-teste-example",
                Key = JoinFolderAndFilename(folderGuid.ToString(), file.FileName),
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            var result = await _amazonS3.PutObjectAsync(request);

            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK) {
                _files3Repository.Add(new Models.FileS3(file.FileName, file.ContentType, folderGuid));
            }

            return Ok(result);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Get([FromQuery] string folder, string fileName) 
        {
            var request = new GetObjectRequest
            {
                BucketName = "s3-teste-example",
                Key = JoinFolderAndFilename(folder, fileName)
            };

            try {
                using GetObjectResponse response = await _amazonS3.GetObjectAsync(request);
                using Stream responseStream = response.ResponseStream;
                var stream = new MemoryStream();
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;

                return new FileStreamResult(stream, response.Headers["Content-type"])
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string folder, string fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = "s3-teste-example",
                Key = JoinFolderAndFilename(folder, fileName)
            };

            try {
                var response = await _amazonS3.DeleteObjectAsync(request);

                return Ok(response);
            }
            catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }

        private string JoinFolderAndFilename(string folder, string filename)
        {
            if (string.IsNullOrEmpty(folder)) return filename;

            return string.Join("/", folder, filename);
        }

        [HttpGet("all-from-db")]
        public IActionResult GetAllFilesS3FromDb()
        {
            var result = _files3Repository.GetAll();

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        [HttpGet("{id}/from-db")]
        public IActionResult GetByIdFromDb(int id)
        {
            var result = _files3Repository.GetById(id);

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }
    }
}