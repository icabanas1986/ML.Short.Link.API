namespace ML.Short.Link.API.Models
{
    public class ClickStats
    {
        public int TotalClicks { get; set; }
        public int UniqueVisitors { get; set; }
        public string TopCountry { get; set; } = string.Empty;
    }
}
