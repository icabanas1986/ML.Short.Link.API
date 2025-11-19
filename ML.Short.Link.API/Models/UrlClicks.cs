using System;

namespace ML.Short.Link.API.Models
{
    public class UrlClicks
    {
        public int Id { get; set; }
        public int UrlId { get; set; }
        public DateTime ClickedAt { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        // Geolocalización extendida
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty; // Ej: "US", "MX"
        public string Region { get; set; } = string.Empty; // Estado/Provincia
        public string City { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Timezone { get; set; } = string.Empty; // Ej: "America/Mexico_City"
        public string PostalCode { get; set; } = string.Empty;

        // Información del dispositivo extendida
        public string DeviceType { get; set; } = string.Empty; // Mobile, Desktop, Tablet
        public string Browser { get; set; } = string.Empty; // Chrome, Firefox, Safari
        public string Platform { get; set; } = string.Empty; // Windows, macOS, Android
        public bool IsMobile { get; set; }
        public bool IsBot { get; set; }

        // Metadata adicional
        public string ISP { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;

    }
}
