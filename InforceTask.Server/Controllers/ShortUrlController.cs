using AutoMapper;
using InforceTask.Server.Attributes;
using InforceTask.Server.Models;
using InforceTask.Server.Models.DTOs;
using InforceTask.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InforceTask.Server.Controllers
{
    [Route("api/shorturls")]
    [ApiController]
    public class ShortUrlsController : ControllerBase
    {
        private readonly IShortUrlRepository _repository;

        private readonly IMapper _mapper;

        private readonly IUrlValidationService _urlValidationService;

        public ShortUrlsController(IShortUrlRepository repository, IMapper mapper, IUrlValidationService urlValidationService)
        {
            _repository = repository;
            _mapper = mapper;
            _urlValidationService = urlValidationService;
        }


        /// <summary>
        /// Retrieves all short URLs.
        /// </summary>
        /// <returns>List of short URLs.</returns>
        /// <response code="200">List of short URLs returned.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ShortUrlListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var urls = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ShortUrlListDto>>(urls);
            var isAdmin = User.IsInRole("Admin");

            // Inject full short URL
            foreach (var dto in result)
            {
                dto.ShortUrl = $"{Request.Scheme}://{Request.Host}/r/{dto.ShortCode}";

                dto.CanEdit = isAdmin || (dto.AuthorEmail != null && dto.AuthorEmail == User.Identity?.Name);
            }


            return Ok(result);
        }

        /// <summary>
        /// Retrieves a short URL by its numeric ID.
        /// </summary>
        /// <param name="id">Numeric ID of the short URL.</param>
        /// <returns>Details of the short URL.</returns>
        /// <response code="200">Short URL details returned.</response>
        /// <response code="404">Short URL not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ShortUrlDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {

            var link = await _repository.GetByIdAsync(id);
            if (link == null)
                return NotFound();

            var dto = _mapper.Map<ShortUrlDetailDto>(link);
            dto.ShortUrl = $"{Request.Scheme}://{Request.Host}/r/{link.ShortCode}";

            return Ok(dto);
        }


        /// <summary>
        /// Creates a new short URL.
        /// </summary>
        /// <param name="originalUrl">The original URL to shorten.</param>
        /// <returns>Details of the created short URL.</returns>
        /// /// <response code="200">Short URL created successfully.</response>
        /// <response code="400">Invalid or duplicate URL provided.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] string originalUrl)
        {
            var (isValid, error) = await _urlValidationService.ValidateUrlAsync(originalUrl);
            if (!isValid)
                return BadRequest(error);

            var authorEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(authorEmail))
                return Unauthorized("Cannot determine author email");

            var existing = await _repository.FindExistingAsync(originalUrl);
            if (existing != null)
            {
                string existingShortLink = $"{Request.Scheme}://{Request.Host}/r/{existing.ShortCode}";
                return BadRequest(new { shortUrl = existingShortLink, message = "URL already exists" });
            }

            string shortCode = Guid.NewGuid().ToString("N")[..6];

            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                AuthorEmail = authorEmail
            };

            await _repository.CreateAsync(shortUrl);

            string shortLink = $"{Request.Scheme}://{Request.Host}/r/{shortCode}";

            return Ok(new { shortUrl = shortLink, message = "New short URL created" });
        }


        /// <summary>
        /// Updates an existing short URL.
        /// </summary>
        /// <param name="id">Numeric ID of the short URL.</param>
        /// <param name="newOriginalUrl">The new original URL.</param>
        /// <returns>Status of the update operation.</returns>
        /// /// <response code="200">Short URL updated successfully.</response>
        /// <response code="400">Invalid URL provided.</response>
        /// <response code="404">Short URL not found.</response>
        /// <response code="403">Forbidden: not the author or an admin.</response>
        [HttpPut("{id:int}")]
        [ShortUrlAuthorOrAdmin]
        public async Task<IActionResult> Update(int id, [FromBody] string newOriginalUrl)
        {
            var (isValid, error) = await _urlValidationService.ValidateUrlAsync(newOriginalUrl);
            if (!isValid)
                return BadRequest(error);

            var shortUrl = await _repository.GetByIdAsync(id);
            if (shortUrl == null)
                return NotFound("Short link not found");

            shortUrl.OriginalUrl = newOriginalUrl;
            await _repository.UpdateAsync(shortUrl);

            return Ok(new { message = "Short URL updated successfully" });
        }

        /// <summary>
        /// Deletes a short URL by its numeric ID.
        /// </summary>
        /// <param name="id">Numeric ID of the short URL.</param>
        /// <returns>Status of the deletion operation.</returns>
        /// /// <response code="200">Short URL deleted successfully.</response>
        /// <response code="404">Short URL not found.</response>
        /// <response code="403">Forbidden: not the author or an admin.</response>
        [HttpDelete("{id:int}")]
        [ShortUrlAuthorOrAdmin]
        public async Task<IActionResult> Delete(int id)
        {
            var shortUrl = await _repository.GetByIdAsync(id);
            if (shortUrl == null)
                return NotFound("Short link not found");

            await _repository.DeleteAsync(shortUrl);

            return Ok(new { message = "Short URL deleted successfully" });
        }

        /// <summary>
        /// Redirects to the original URL for a given short code.
        /// </summary>
        /// <param name="code">The short code of the URL.</param>
        /// <returns>Redirects to the original URL if found.</returns>
        /// /// <response code="302">Redirect to original URL.</response>
        /// <response code="404">Short URL not found.</response>
        [HttpGet("/r/{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var shortUrl = await _repository.GetByCodeAsync(code);
            if (shortUrl == null)
                return NotFound("Short link not found");

            return Redirect(shortUrl.OriginalUrl);
        }
    }
}
