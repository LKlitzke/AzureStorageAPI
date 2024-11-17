using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public ArquivosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("StorageAccountConnectionString");
            _containerName = configuration.GetValue<string>("BlobContainerName");
        }

        [HttpPost("Upload")]
        public IActionResult UploadArquivo(IFormFile file)
        {
            BlobContainerClient container = new (_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient(file.FileName);

            using var data = file.OpenReadStream();
            blobClient.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            });

            return Ok(blobClient.Uri.ToString());
        }

        [HttpGet("Download/{name}")]
        public IActionResult DownloadArquivo(string name)
        {
            BlobContainerClient container = new(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient(name);

            if (!blobClient.Exists()) return BadRequest();
            var result = blobClient.DownloadContent();
            return File(result.Value.Content.ToArray(), result.Value.Details.ContentType, blobClient.Name);
        }

        [HttpDelete("Delete/{name}")]
        public IActionResult DeleteArquivo(string name)
        {
            BlobContainerClient container = new(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient(name);

            var result = blobClient.Delete();
            return NoContent();
        }

        [HttpGet("Listar")]
        public IActionResult ListarArquivos()
        {
            List<BlobDto> list = new List<BlobDto>();


            BlobContainerClient container = new(_connectionString, _containerName);

            foreach(var blob in container.GetBlobs())
            {
                list.Add(new BlobDto { Nome = blob.Name, Tipo = blob.Properties.ContentType, Uri = container.Uri.AbsoluteUri + "/" + blob.Name });
            }

            return Ok(list);
        }
    }
}