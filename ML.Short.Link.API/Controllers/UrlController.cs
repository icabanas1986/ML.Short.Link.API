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
        private readonly IConfiguration _config;
        public UrlController( UrlShortenerService shortener, IConfiguration config)
        {
            _shortener = shortener;
            _config = config;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            
            var urlDefault = _config.GetValue<string>("UrlDefault:url");
            var shortCode = _shortener.GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                IdUser = 1,
            };

            var idUrl = await _shortener.InsertaUrlAsync(shortUrl);
            var shorty = urlDefault + shortCode;
            return Ok(new { shortUrl = shorty });
            
        }

        [HttpPost]
        [Route("free")]
        public async Task<IActionResult> ShortenUrlFree([FromBody] string originalUrl)
        {
            var urlDefault = _config.GetValue<string>("UrlDefault:url");
            var shortCode = _shortener.GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                IdUser = 1,
            };
            var idUrl = await _shortener.InsertaUrlAsync(shortUrl);
            var shorty = urlDefault + shortCode;
            return Ok(new { shortUrl = shorty });
        }
    }
}
