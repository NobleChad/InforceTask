using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InforceTask.Server.Models
{
    public class ShortUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        public string ShortCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string AuthorEmail { get; set; } 
    }
}
