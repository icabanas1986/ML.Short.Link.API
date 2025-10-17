namespace ML.Short.Link.API.Models
{
    public class DeviceInfo
    {
        public string DeviceType { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public bool IsMobile { get; set; }
        public bool IsBot { get; set; }
    }
}
