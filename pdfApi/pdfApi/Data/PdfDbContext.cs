using Microsoft.EntityFrameworkCore;
using pdfApi.Models;

namespace pdfApi.Data
{
    public class PdfDbContext : DbContext
    {
        public PdfDbContext(DbContextOptions<PdfDbContext> options)
            : base(options)
        {
        }

        public DbSet<PdfFile> PdfFiles { get; set; }
    }
}
