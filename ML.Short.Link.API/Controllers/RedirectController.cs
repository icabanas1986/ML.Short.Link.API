using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ML.Short.Link.API.Services;

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
            if (string.IsNullOrWhiteSpace(url))
            {
                return NotFound();
            }
            return Redirect(url);
        }
    }
}
