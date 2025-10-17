using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ML.Short.Link.API.Services;
using ML.Short.Link.API.Utils;
using ML.Short.Link.API.Utils.Interface;
using System.Drawing;
using System.Security.Cryptography;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController : ControllerBase
    {
        private readonly UrlShortenerService _shortener;
        private readonly UrlClickServices _clickServices;
        public RedirectController(UrlShortenerService shortener, UrlClickServices clickServices)
        {
            SimpleLogger.LogInfo("RedirectController", nameof(RedirectController));
            _shortener = shortener;
            _clickServices = clickServices;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectUrl(string shortCode)
        {
            
            var url = await _shortener.ObtenerUrlOriginalAsync(shortCode);
            SimpleLogger.LogInfo("RedirectController", $"Redirigiendo a: {url}");
            if (url == null)
            {
                return BadRequest("No se encontro url original.");
            }

            //Obtenemos la información de url original
            var originalUrl = await _shortener.ObtenerInfoUrlOriginalAsync(shortCode);
            SimpleLogger.LogInfo("RedirectController", $"Info de la url original obtenida: {originalUrl?.OriginalUrl}");

            var infoGuardada = await _clickServices.ProcesaDatosTrackin(HttpContext,originalUrl);
            SimpleLogger.LogInfo("RedirectController", $"Info de tracking guardada: {infoGuardada}");

            var randomNumberGenerator = RandomNumberGenerator.Create();
            var secretKey = new byte[32];
            randomNumberGenerator.GetBytes(secretKey);
            SimpleLogger.LogInfo("RedirectController", $"Clave secreta generada: {Convert.ToBase64String(secretKey)}");

            var realSecretKey = Convert.ToBase64String(secretKey);

            if (string.IsNullOrWhiteSpace(url))
            {
                return NotFound();
            }
            return Redirect(url);
        }
    }
}
