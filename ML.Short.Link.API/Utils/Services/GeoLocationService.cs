using Microsoft.Extensions.Caching.Memory;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils.Interface;
using System.Net;
using System.Security.Cryptography.Xml;

namespace ML.Short.Link.API.Utils.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;
        public GeoLocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeoLocationInfo?> GetLocationFromIPAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)|| IsPrivateIP(ipAddress))
            {
                SimpleLogger.LogInfo("GeoLocationService", "IP es local o inválida, retornando ubicación local.");
                return new GeoLocationInfo { Country = "Local", CountryCode = "LOCAL" };
            }
            try
            {
                var geoInfo =  await GetFromIpApi(ipAddress);
                return 
                    geoInfo;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("GeoLocationService", ex, "Error al obtener geolocalización");
                return await GetFromIpApi(ipAddress);
            }
        }

        private async Task<GeoLocationInfo> GetFromIpApi(string ipAddress)
        {
            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>($"http://ip-api.com/json/{ipAddress}");
           SimpleLogger.LogInfo("GeoLocationService", $"Respuesta de ipapi.co: {response}");
            return new GeoLocationInfo
            {
                Country = response.CountryName ?? "Unknown",
                CountryCode = response.CountryCode ?? "UNK",
                Region = response.Region ?? string.Empty,
                City = response.City ?? string.Empty,
                Latitude = response.Latitude,
                Longitude = response.Longitude,
                Timezone = response.Timezone ?? string.Empty,
                ISP = response.Org ?? string.Empty,
                Organization = response.Org ?? string.Empty,
                PostalCode = response.Postal ?? string.Empty
            };
            throw new Exception("Failed to retrieve geolocation data from ip-api.com");
        }
        private bool IsPrivateIP(string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out var ip))
            {
                // Verificar si es IP local/privada
                if (IPAddress.IsLoopback(ip)) return true;

                // Rangos de IPs privadas
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    var bytes = ip.GetAddressBytes();
                    return (bytes[0] == 10) ||
                           (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                           (bytes[0] == 192 && bytes[1] == 168);
                }
            }
            return false;
        }
    }
}
