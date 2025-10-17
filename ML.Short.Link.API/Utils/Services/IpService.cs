using ML.Short.Link.API.Utils.Interface;
using System.Net;

namespace ML.Short.Link.API.Utils.Services
{
    public class IpService: IIpService
    {
        public string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = GetClientIPAddress(context);
            return ipAddress?.ToString() ?? "Unknown";
        }

        public IPAddress GetClientIPAddress(HttpContext context)
        {
            // Primero verificar headers de proxy (X-Forwarded-For)
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var xForwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(xForwardedFor))
                {
                    // Puede contener múltiples IPs, la primera es la del cliente
                    var ip = xForwardedFor.Split(',')[0].Trim();
                    if (IPAddress.TryParse(ip, out var ipAddress))
                        return ipAddress;
                }
            }

            // Headers comunes de proxies
            var headers = new[] { "X-Real-IP", "X-Client-IP", "CF-Connecting-IP" };
            foreach (var header in headers)
            {
                if (context.Request.Headers.ContainsKey(header))
                {
                    var ip = context.Request.Headers[header].ToString();
                    if (IPAddress.TryParse(ip, out var ipAddress))
                        return ipAddress;
                }
            }

            // Conexión directa
            return context.Connection.RemoteIpAddress;
        }
    }
}
