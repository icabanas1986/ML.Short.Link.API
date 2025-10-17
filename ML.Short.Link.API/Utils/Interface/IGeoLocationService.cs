using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Utils.Interface
{
    public interface IGeoLocationService
    {
        Task<GeoLocationInfo> GetLocationFromIPAsync(string ipAddress);
    }
}
