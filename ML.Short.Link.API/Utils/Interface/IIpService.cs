using System.Net;

namespace ML.Short.Link.API.Utils.Interface
{
    public interface IIpService
    {
        string GetClientIpAddress(HttpContext context);
        IPAddress GetClientIPAddress(HttpContext context);
    }
}
