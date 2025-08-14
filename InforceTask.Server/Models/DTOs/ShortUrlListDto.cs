namespace InforceTask.Server.Models.DTOs
{
    public class ShortUrlListDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public bool CanEdit { get; set; }
    }
}
