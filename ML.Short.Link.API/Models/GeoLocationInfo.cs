namespace ML.Short.Link.API.Models
{
    public class GeoLocationInfo
    {
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public string ISP { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
