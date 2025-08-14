namespace InforceTask.Server.Models.DTOs
{
    public class ShortUrlDetailDto
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorEmail { get; set; } = string.Empty;
    }
}
