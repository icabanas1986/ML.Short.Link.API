using Azure.Core;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils;
using ML.Short.Link.API.Utils.Interface;

namespace ML.Short.Link.API.Services
{
    public class UrlClickServices
    {
        private readonly IUrlClicksRepository _db;
        private readonly IGeoLocationService _geoLocationService;
        private readonly IDeviceDetectionService _deviceDetectionService;

        public UrlClickServices(IUrlClicksRepository db, IGeoLocationService geoLocationService,
        IDeviceDetectionService deviceDetectionService)
        {
            _db = db;
            _geoLocationService = geoLocationService;
            _deviceDetectionService = deviceDetectionService;
        }


        public async Task<bool> ProcesaDatosTrackin(HttpContext context,ShortUrl urlClicks)
        {
            UrlClicks urlClick = new UrlClicks();
            var infoGuardada = false;
            try
            {
                SimpleLogger.LogInfo("UrlClickServices", "Iniciando proceso de tracking");
                var ipAddress = GetClientIPAddress(context);
                //ipAddress = "201.137.48.194";
                var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
                SimpleLogger.LogInfo("UrlClickServices", $"IP Address: {ipAddress}, User-Agent: {userAgent}");


                //obtemos informacion de geolocalizacion
                var geoLocationTask = _geoLocationService.GetLocationFromIPAsync(ipAddress);
                SimpleLogger.LogInfo("UrlClickServices", "Solicitando geolocalización");

                var deviceInfoTask = Task.Run(() => _deviceDetectionService.GetDeviceInfo(userAgent));
                SimpleLogger.LogInfo("UrlClickServices", "Solicitando información del dispositivo");


                await Task.WhenAll(geoLocationTask, deviceInfoTask);

                var geoLocation = await geoLocationTask;
                var deviceInfo = await deviceInfoTask;

                urlClick.UrlId = urlClicks.Id;
                urlClick.ClicketAt = DateTime.UtcNow;
                urlClick.IPAddress = ipAddress;
                urlClick.UserAgent = userAgent;

                // Asignar datos de geolocalización
                urlClick.Country = geoLocation.Country;
                urlClick.CountryCode = geoLocation.CountryCode;
                urlClick.Region = geoLocation.Region;
                urlClick.City = geoLocation.City;
                urlClick.Latitude = geoLocation.Latitude;
                urlClick.Longitude = geoLocation.Longitude;
                urlClick.Timezone = geoLocation.Timezone;
                urlClick.ISP = geoLocation.ISP;
                urlClick.Organization = geoLocation.Organization;
                urlClick.PostalCode = geoLocation.PostalCode;

                // Asignar datos del dispositivo
                urlClick.DeviceType = deviceInfo.DeviceType;
                urlClick.Browser = deviceInfo.Browser;
                urlClick.Platform = deviceInfo.Platform;
                urlClick.IsMobile = deviceInfo.IsMobile;
                urlClick.IsBot = deviceInfo.IsBot;

                infoGuardada = await _db.IngresaClicks(urlClick);
                SimpleLogger.LogInfo("UrlClickServices", $"Información de tracking guardada: {infoGuardada}");
            }
            catch (Exception ex)
            {
                return false;
            }

            return infoGuardada;
        }

        private string GetClientIPAddress(HttpContext httpContext)
        {

            // Considerar headers de proxy
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                return httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();

            if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
                return httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public async Task<bool> InsertaUrlClicks(UrlClicks u)
        {
            var insertado = false;
            try
            {
                insertado = await _db.IngresaClicks(u);
            }
            catch (Exception ex)
            {
                var menaje = ex.Message;
            }
            return insertado;
        }
    }
}
