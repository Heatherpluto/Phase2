using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using pdfApi.Controllers;
using pdfApi.Data;
using pdfApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace pdfApi.Tests.Controllers
{
    public class PdfFilesControllerTests
    {
        private readonly PdfFilesController _controller;
        private readonly Mock<ILogger<PdfFilesController>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PdfDbContext _context;

        public PdfFilesControllerTests()
        {
            _loggerMock = new Mock<ILogger<PdfFilesController>>();
            _configurationMock = new Mock<IConfiguration>();

            var options = new DbContextOptionsBuilder<PdfDbContext>()
                .UseInMemoryDatabase(databaseName: "PdfFilesDatabase")
                .Options;
            _context = new PdfDbContext(options);

            _controller = new PdfFilesController(_configurationMock.Object, _context, _loggerMock.Object);
        }

        [Fact]
        public async Task SearchByFilename_ReturnsNotFound_WhenNoFilesMatch()
        {
            // Act
            var result = await _controller.SearchByFilename("nonexistent");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SearchByFilename_ReturnsFiles_WhenFilesMatch()
        {
            // Arrange
            var pdfFile = new PdfFile { Id = 1, FileName = "testfile.pdf", FilePath = "http://example.com/testfile.pdf", Summary = "Test summary" };
            _context.PdfFiles.Add(pdfFile);
            _context.SaveChanges();

            // Act
            var result = await _controller.SearchByFilename("testfile");

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<PdfFile>>>(result);
            var files = Assert.IsType<List<PdfFile>>(okResult.Value);
            Assert.Single(files);
            Assert.Equal(pdfFile.Id, files[0].Id);
        }

        [Fact]
        public async Task PostPdfFile_ReturnsBadRequest_WhenNoFileUploaded()
        {
            // Act
            var result = await _controller.PostPdfFile(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetPdfFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Act
            var result = await _controller.GetPdfFile(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPdfFile_ReturnsFile_WhenFileExists()
        {
            // Arrange
            var pdfFile = new PdfFile { Id = 1, FileName = "test.pdf", FilePath = "http://example.com/test.pdf", Summary = "Test summary" };
            _context.PdfFiles.Add(pdfFile);
            _context.SaveChanges();

            // Act
            var result = await _controller.GetPdfFile(1);

            // Assert
            var okResult = Assert.IsType<ActionResult<PdfFile>>(result);
            var returnValue = Assert.IsType<PdfFile>(okResult.Value);
            Assert.Equal(pdfFile.Id, returnValue.Id);
            Assert.Equal(pdfFile.FileName, returnValue.FileName);
            Assert.Equal(pdfFile.FilePath, returnValue.FilePath);
            Assert.Equal(pdfFile.Summary, returnValue.Summary);
        }

        [Fact]
        public async Task PutPdfFile_ReturnsNoContent_WhenFileUpdated()
        {
            // Arrange
            var pdfFile = new PdfFile { Id = 1, FileName = "test.pdf", FilePath = "http://example.com/test.pdf", Summary = "Test summary" };
            _context.PdfFiles.Add(pdfFile);
            _context.SaveChanges();

            pdfFile.FileName = "updated.pdf";

            // Act
            var result = await _controller.PutPdfFile(1, pdfFile);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePdfFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Act
            var result = await _controller.DeletePdfFile(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePdfFile_ReturnsNoContent_WhenFileDeleted()
        {
            // Arrange
            var pdfFile = new PdfFile { Id = 1, FileName = "test.pdf", FilePath = "http://example.com/test.pdf", Summary = "Test summary" };
            _context.PdfFiles.Add(pdfFile);
            _context.SaveChanges();

            // Act
            var result = await _controller.DeletePdfFile(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(_context.PdfFiles.Any(e => e.Id == 1));
        }
    }
}
