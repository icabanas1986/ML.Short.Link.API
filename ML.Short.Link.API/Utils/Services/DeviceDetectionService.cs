using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils.Interface;

namespace ML.Short.Link.API.Utils.Services
{
    public class DeviceDetectionService : IDeviceDetectionService
    {
        public DeviceInfo GetDeviceInfo(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return new DeviceInfo { DeviceType = "Unknown", Browser = "Unknown", Platform = "Unknown" };

            var deviceInfo = new DeviceInfo
            {
                DeviceType = DetectDeviceType(userAgent),
                Browser = DetectBrowser(userAgent),
                Platform = DetectPlatform(userAgent),
                IsMobile = DetectIsMobile(userAgent),
                IsBot = DetectIsBot(userAgent)
            };
            SimpleLogger.LogInfo("DeviceDetectionService", $"Detected Device Info: {deviceInfo.DeviceType}, {deviceInfo.Browser}, {deviceInfo.Platform}, IsMobile: {deviceInfo.IsMobile}, IsBot: {deviceInfo.IsBot}");
            return deviceInfo;
        }

        private string DetectDeviceType(string userAgent)
        {
            var ua = userAgent.ToLowerInvariant();

            if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
                return "Mobile";
            if (ua.Contains("tablet") || ua.Contains("ipad"))
                return "Tablet";
            if (ua.Contains("tv") || ua.Contains("smart-tv"))
                return "TV";
            if (ua.Contains("bot") || ua.Contains("crawler"))
                return "Bot";

            return "Desktop";
        }

        private string DetectBrowser(string userAgent)
        {
            var ua = userAgent.ToLowerInvariant();

            if (ua.Contains("chrome") && !ua.Contains("edg/")) return "Chrome";
            if (ua.Contains("firefox")) return "Firefox";
            if (ua.Contains("safari") && !ua.Contains("chrome")) return "Safari";
            if (ua.Contains("edg/")) return "Edge";
            if (ua.Contains("opera")) return "Opera";
            if (ua.Contains("brave")) return "Brave";

            return "Other";
        }

        private string DetectPlatform(string userAgent)
        {
            var ua = userAgent.ToLowerInvariant();

            if (ua.Contains("windows")) return "Windows";
            if (ua.Contains("mac os")) return "macOS";
            if (ua.Contains("linux")) return "Linux";
            if (ua.Contains("android")) return "Android";
            if (ua.Contains("iphone") || ua.Contains("ipad")) return "iOS";

            return "Unknown";
        }

        private bool DetectIsMobile(string userAgent)
        {
            var ua = userAgent.ToLowerInvariant();
            return ua.Contains("mobile") ||
                   ua.Contains("android") ||
                   ua.Contains("iphone") ||
                   ua.Contains("ipad");
        }

        private bool DetectIsBot(string userAgent)
        {
            var ua = userAgent.ToLowerInvariant();
            var botIndicators = new[]
            {
            "bot", "crawler", "spider", "facebookexternalhit",
            "twitterbot", "googlebot", "bingbot", "slurp",
            "duckduckbot", "baiduspider", "yandexbot", "sogou"
        };

            return botIndicators.Any(indicator => ua.Contains(indicator));
        }
    }
}
