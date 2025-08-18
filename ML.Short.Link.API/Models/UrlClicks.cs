using StackExchange.Redis;

namespace ML.Short.Link.API.Models
{
    public class UrlClicks
    {
        public int Id { get; set; }
        public int UrlId { get; set; }
        public DateTime ClickedAt { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Country { get; set; }
        public string? DeviceType { get; set; }
        public string ShortCode { get; set; }
    }
}
