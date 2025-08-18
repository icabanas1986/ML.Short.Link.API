using Microsoft.AspNetCore.Mvc;
using ML.Short.Link.API.Services;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("list")]
    public class ListController : Controller
    {
        private readonly UrlShortenerService _shortener;

        public ListController(UrlShortenerService shortener)
        {
            _shortener = shortener;
        }
        [HttpGet]
        public async Task<IActionResult> list(int userId)
        {
            var lista = await _shortener.ObtenerUrlsPorUsuarioAsync(userId);
            return Ok(lista);
        }
    }
}
