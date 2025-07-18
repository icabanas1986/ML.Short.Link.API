using Microsoft.AspNetCore.Mvc;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Services;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlController : Controller
    {
        private readonly UrlShortenerService _shortener;

        public UrlController( UrlShortenerService shortener)
        {
            _shortener = shortener;
        }

        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            var shortCode = _shortener.GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode
            };

            //_db.ShortUrls.Add(shortUrl);
            //await _db.SaveChangesAsync();

            return Ok(new { shortUrl = $"https://lish.io/{shortCode}" });
        }
    }
}
