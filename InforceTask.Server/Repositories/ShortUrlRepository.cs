using InforceTask.Server.Data;
using InforceTask.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTask.Server.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly ApplicationDbContext _context;

        public ShortUrlRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShortUrl>> GetAllAsync() =>
            await _context.ShortUrls.ToListAsync();

        public async Task<ShortUrl?> GetByIdAsync(int id) =>
            await _context.ShortUrls.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<ShortUrl?> GetByCodeAsync(string code) =>
            await _context.ShortUrls.FirstOrDefaultAsync(s => s.ShortCode == code);

        public async Task<ShortUrl?> FindExistingAsync(string originalUrl) =>
            await _context.ShortUrls
                .FirstOrDefaultAsync(s => s.OriginalUrl == originalUrl);

        public async Task<ShortUrl> CreateAsync(ShortUrl shortUrl)
        {
            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();
            return shortUrl;
        }

        public async Task UpdateAsync(ShortUrl shortUrl)
        {
            _context.ShortUrls.Update(shortUrl);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ShortUrl shortUrl)
        {
            _context.ShortUrls.Remove(shortUrl);
            await _context.SaveChangesAsync();
        }
    }
}
