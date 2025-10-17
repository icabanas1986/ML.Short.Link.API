using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Utils.Interface
{
    public interface IDeviceDetectionService
    {
        DeviceInfo GetDeviceInfo(string userAgent);
    }
}
