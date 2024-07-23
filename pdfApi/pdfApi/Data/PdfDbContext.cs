using System;
using Microsoft.EntityFrameworkCore;
using PdfUploader.Models;

namespace pdfApi.Data
{
	public class PdfDbContext : DbContext
    {
        public PdfDbContext(DbContextOptions<PdfDbContext> options)
     : base(options)
        {
        }

        public DbSet<PdfFile> PdfFile { get; set; }
    }
}

