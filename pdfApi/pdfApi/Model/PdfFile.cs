namespace PdfUploader.Models
{
    public class PdfFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }
}
