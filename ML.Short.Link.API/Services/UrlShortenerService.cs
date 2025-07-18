using System.Security.Cryptography;
using System.Text;

namespace ML.Short.Link.API.Services
{
    public class UrlShortenerService
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new();

        public string GenerateShortCode(int length = 6)
        {
            return new string(Enumerable.Repeat(Alphabet, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
