using System.Security.Principal;

namespace ML.Short.Link.API.Models
{
    public class ShortUrlModel
    {
        public string ShortUrl { get; set; }
        public int idUser { get; set; }
    }
}
