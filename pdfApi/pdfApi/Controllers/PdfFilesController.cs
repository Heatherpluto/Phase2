using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using OpenAI_API.Chat;
using pdfApi.Data;
using pdfApi.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace pdfApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfFilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PdfDbContext _context;
        private readonly ILogger<PdfFilesController> _logger;
        private readonly string _blobConnectionString;
        private readonly string _openAIapiKey;
        private const string containerName = "msa2";

        public PdfFilesController(IConfiguration configuration, PdfDbContext context, ILogger<PdfFilesController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _blobConnectionString = _configuration.GetConnectionString("blobConnectionString");
            _openAIapiKey = _configuration["OpenAI:ApiKey"];
        }

        [HttpGet("SearchByFilename")]
        public async Task<ActionResult<IEnumerable<PdfFile>>> SearchByFilename(string filename)
        {
            var pdfFiles = await _context.PdfFiles
                .Where(p => p.FileName.Contains(filename))
                .ToListAsync();

            if (!pdfFiles.Any())
            {
                return NotFound();
            }

            return pdfFiles;
        }


        [HttpPost("PostPdfFile")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PdfFile>> PostPdfFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var (uploadSuccess, blobUrl) = await UploadToBlob(file);
            if (!uploadSuccess)
                return StatusCode(500, "Error uploading to blob storage.");

            string extractedText = ExtractTextFromPdf(file.OpenReadStream());
            string summary = await GenerateSummary(extractedText);

            var pdfFile = new PdfFile
            {
                FileName = file.FileName,
                FilePath = blobUrl,
                Summary = summary
            };

            _context.PdfFiles.Add(pdfFile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPdfFile", new { id = pdfFile.Id }, pdfFile);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PdfFile>> GetPdfFile(int id)
        {
            var pdfFile = await _context.PdfFiles.FindAsync(id);

            if (pdfFile == null)
                return NotFound();

            return pdfFile;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPdfFile(int id, PdfFile pdfFile)
        {
            if (id != pdfFile.Id)
            {
                return BadRequest();
            }

            _context.Entry(pdfFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoesPdfFileExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool DoesPdfFileExist(int id)
        {
            return _context.PdfFiles.Any(e => e.Id == id);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePdfFile(int id)
        {
            var pdfFile = await _context.PdfFiles.FindAsync(id);
            if (pdfFile == null)
                return NotFound();

            await DeleteBlob(pdfFile.FileName);
            _context.PdfFiles.Remove(pdfFile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PdfFileExists(int id)
        {
            return _context.PdfFiles.Any(e => e.Id == id);
        }

        private async Task<(bool Success, string BlobUrl)> UploadToBlob(IFormFile file)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return (true, blobClient.Uri.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Blob Storage");
                return (false, null);
            }
        }

        private async Task DeleteBlob(string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            using (var document = PdfDocument.Open(pdfStream))
            {
                return string.Join(" ", document.GetPages().Select(page => page.Text));
            }
        }

        private async Task<string> GenerateSummary(string text)
        {
            try
            {
                APIAuthentication aPIAuthentication = new APIAuthentication(_openAIapiKey);
                OpenAIAPI openAiApi = new OpenAIAPI(aPIAuthentication);

                string model = "gpt-3.5-turbo";
                int maxTokens = 1000;

                var chatRequest = new ChatRequest
                {
                    Model = model,
                    Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = ChatMessageRole.User,
                    Content = $"Summarize the following text:\n\n{text}"
                }
            },
                    MaxTokens = maxTokens
                };

                var chatResponse = await openAiApi.Chat.CreateChatCompletionAsync(chatRequest);

                if (chatResponse.Choices == null || !chatResponse.Choices.Any())
                {
                    _logger.LogError("No completions found in the OpenAI API response.");
                    return "Error calling OpenAI API: No completions found.";
                }

                return chatResponse.Choices[0].Message.Content.Trim();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API: {Message}\n{StackTrace}", ex.Message, ex.StackTrace);
                return $"Error calling OpenAI API: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API: {Message}\n{StackTrace}", ex.Message, ex.StackTrace);
                return $"Error calling OpenAI API: {ex.Message}";
            }
        }


    }
}
