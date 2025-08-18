using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Model;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ML.Short.Link.API.Services
{
    public class UrlShortenerService
    {
        private readonly IUrlRepositorio _db;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DatabaseReader _reader;
        

        public UrlShortenerService(IUrlRepositorio db, IHttpContextAccessor httpContextAccessor, DatabaseReader reader)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _reader = reader;
        }
        public string GenerateShortCode(int length = 6)
        {
            return new string(Enumerable.Repeat(Alphabet, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task<int> InsertaUrlAsync(ShortUrl shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl.OriginalUrl) || string.IsNullOrWhiteSpace(shortUrl.ShortCode))
            {
                throw new ArgumentException("Original URL and short code cannot be null or empty.");
            }
            return await _db.InsertarUrlAsync(shortUrl.OriginalUrl, shortUrl.ShortCode, shortUrl.IdUser);
        }

        public async Task<string> ObtenerUrlOriginalAsync(UrlClicks shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode.ShortCode))
            {
                throw new ArgumentException("Short code cannot be null or empty.");
            }
            var shorUrl = await _db.ObtenerUrlOriginalAsync(shortCode.ShortCode);
            if (string.IsNullOrWhiteSpace(shorUrl.OriginalUrl))
            {
                throw new KeyNotFoundException("Short code not found.");
            }
            await _db.IncrementarClickCountAsync(shortCode.ShortCode);
            shortCode.UrlId = shorUrl.Id;
            var intClicks = await GetStats(shortCode);
            
            return shorUrl.OriginalUrl;

        }
        public async Task<int> GetStats(UrlClicks urlClicks)
        {
            
            if (urlClicks.UrlId <= 0)
            {
                throw new ArgumentException("URL ID must be greater than zero.");
            }
            
            string country = "Desconocido";
            var devideType = GetDeviceType(urlClicks.UserAgent);

            if (_reader != null && IPAddress.TryParse(urlClicks.IPAddress, out var ip))
            {
                try
                { 
                    var countryResponse = _reader.City(ip);
                    country = countryResponse.Country.Name ?? "Desconocido";
                }
                catch { /* Ignorar errores */ }
            }
            urlClicks.ClickedAt = DateTime.UtcNow;
            urlClicks.Country = country;
            urlClicks.DeviceType = GetDeviceType(urlClicks.UserAgent);
            int idUrlClicks = await _db.InsertStats(urlClicks);
            return idUrlClicks;
        }
        private string GetDeviceType(string userAgent)
        {
            if (userAgent.Contains("Mobile") || userAgent.Contains("Android"))
            {
                return "Mobile";
            }
            else if (userAgent.Contains("iPad") || userAgent.Contains("Tablet"))
            {
                return "Tablet";
            }
            else
            {
                return "Desktop";
            }
        }
        public async Task<List<ShortUrl>> ObtenerUrlsPorUsuarioAsync(int idUser)
        {
            if (idUser == 0)
            {
                throw new ArgumentException("User ID must be greater than zero.");
            }
            return await _db.ObtenerUrlsPorUsuarioAsync(idUser);

        }
    }
}
