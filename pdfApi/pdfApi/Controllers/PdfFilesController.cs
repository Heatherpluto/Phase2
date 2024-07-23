using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using PdfUploader.Models;

namespace pdfApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfFilesController : ControllerBase
    {

        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountmsap2;AccountKey=Fnj22pHQ4XjDCXQgoMIBnj2rCgO/y0JOHl7k8dcSsSvUPwbwyjruryDSOaKZHGGDYORXUM+Z83U++AStr86sww==;EndpointSuffix=core.windows.net";
        private const string containerName = "msa2";


        [HttpPost("PostPdfFile")]
        public async Task<ActionResult<PdfFile>> PostPdfFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            await UploadToBlob(file);

            return Ok();
        }




        private async Task UploadToBlob(IFormFile file)
        {

            var sqlConnection = "Server=tcp:heathser.database.windows.net,1433;Initial Catalog=PdfUploaderDB;Persist Security Info=False;User ID=CloudSAc51f2e3c;Password=Msatest2024;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            try
            {
                // Create a BlobServiceClient object
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Create a BlobContainerClient object
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Create the container if it does not exist
                await containerClient.CreateIfNotExistsAsync();

                // Get a reference to the blob
                BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

                // Upload the file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}

