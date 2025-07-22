using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public RedirectController(UrlShortenerService shortener)
        {
            _shortener = shortener;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectUrl(string shortCode)
        {
            var url = await _shortener.ObtenerUrlOriginalAsync(shortCode);

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
