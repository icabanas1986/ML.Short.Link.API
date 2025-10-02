using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace ML.Short.Link.API.Services
{
    public class UrlShortenerService
    {
        private readonly IUrlRepositorio _db;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new();

        public UrlShortenerService(IUrlRepositorio db)
        {
            _db = db;
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

        public async Task<string> ObtenerUrlOriginalAsync(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                throw new ArgumentException("Short code cannot be null or empty.");
            }
            var urlOriginal = await _db.ObtenerUrlOriginalAsync(shortCode);
            if (string.IsNullOrWhiteSpace(urlOriginal))
            {
                throw new KeyNotFoundException("Short code not found.");
            }
            await _db.IncrementarClickCountAsync(shortCode);
            return urlOriginal;

        }

        public async Task<List<ShortUrl>> ObtieneUrlsUsuario(int idUser)
        {
            if (idUser <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }
            return await _db.ObtieneUrlsPorUsuario(idUser);
        }

    }
}
