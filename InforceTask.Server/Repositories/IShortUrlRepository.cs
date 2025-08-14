using InforceTask.Server.Models;

namespace InforceTask.Server.Repositories
{
    public interface IShortUrlRepository
    {
        Task<IEnumerable<ShortUrl>> GetAllAsync();
        Task<ShortUrl?> GetByIdAsync(int id);
        Task<ShortUrl?> GetByCodeAsync(string code);
        Task<ShortUrl?> FindExistingAsync(string originalUrl);
        Task<ShortUrl> CreateAsync(ShortUrl shortUrl);
        Task UpdateAsync(ShortUrl shortUrl);
        Task DeleteAsync(ShortUrl shortUrl);
    }
}
