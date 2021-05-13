using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FileToS3Storage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileToS3Storage.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IFileS3Service _fileS3Service;

        public FileController(IAmazonS3 amazonS3, IFileS3Service fileS3Service)
        {
            _amazonS3 = amazonS3;
            _fileS3Service = fileS3Service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var result = await _fileS3Service.SaveToS3(file);

            return Ok(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Get(int id) 
        {
            try {
                var result = await _fileS3Service.DownloadFromS3ById(id);
                
                return result;
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
            var result = _fileS3Service.GetAllFromDb();

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        [HttpGet("{id}/from-db")]
        public IActionResult GetByIdFromDb(int id)
        {
            var result = _fileS3Service.GetByIdFromDb(id);

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }
    }
}