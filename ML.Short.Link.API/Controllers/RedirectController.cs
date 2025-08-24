using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Services;
using System.Drawing;
using System.Security.Cryptography;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController : ControllerBase
    {
        private readonly UrlShortenerService _shortener;
        private readonly IHttpContextAccessor _context;
        //private readonly ILogger<RedirectController> _logger;
        public RedirectController(UrlShortenerService shortener, IHttpContextAccessor context)
        {
            _shortener = shortener;
            _context = context;
            
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectUrl(string shortCode)
        {
            UrlClicks urlClicks = new UrlClicks();

            urlClicks.IPAddress = _context.HttpContext?.Connection.RemoteIpAddress?.ToString();
            //_logger.LogInformation("Obteniendo IP:" + urlClicks.IPAddress, "Redirect", DateTime.UtcNow);
            urlClicks.UserAgent = Request.Headers["User-Agent"].ToString();
            //_logger.LogInformation("Obteniendo UserAgent:" + urlClicks.UserAgent, "Redirect", DateTime.UtcNow);
            urlClicks.ShortCode = shortCode;

            var url = await _shortener.ObtenerUrlOriginalAsync(urlClicks);

            var randomNumberGenerator = RandomNumberGenerator.Create();
            var secretKey = new byte[32];
            randomNumberGenerator.GetBytes(secretKey);
            
            var realSecretKey = Convert.ToBase64String(secretKey);

            if (string.IsNullOrWhiteSpace(url))
            {
                return NotFound();
            }
            return Redirect(url);
        }
    }
}
