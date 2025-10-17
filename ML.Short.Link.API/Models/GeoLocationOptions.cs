namespace ML.Short.Link.API.Models
{
    public class GeoLocationOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Provider { get; set; } = "ipapi"; // ipapi, ipinfo, ipapi-com
        public int TimeoutSeconds { get; set; } = 5;
        public bool CacheEnabled { get; set; } = true;
        public int CacheDurationMinutes { get; set; } = 60;
    }
}
