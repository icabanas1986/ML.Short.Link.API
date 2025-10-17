using System.Text.Json.Serialization;

namespace ML.Short.Link.API.Models
{
    public class IpApiResponse
    {
        [JsonPropertyName("country_name")]
        public string CountryName { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("org")]
        public string Org { get; set; } = string.Empty;

        [JsonPropertyName("postal")]
        public string Postal { get; set; } = string.Empty;
    }
}
