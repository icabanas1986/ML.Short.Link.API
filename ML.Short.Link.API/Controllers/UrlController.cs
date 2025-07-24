using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ML.Short.Link.API.Data;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Services;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("short")]
    public class UrlController : ControllerBase
    {
        private readonly UrlShortenerService _shortener;
        public UrlController( UrlShortenerService shortener)
        {
            _shortener = shortener;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            

            var shortCode = _shortener.GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                IdUser = 1,
            };

            var idUrl = await _shortener.InsertaUrlAsync(shortUrl);

            return Ok(new { shortUrl = $"https://lish.io/{shortCode}" });
        }
    }
}
