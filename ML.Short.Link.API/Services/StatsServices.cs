using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Services
{
    public class StatsServices
    {
        private readonly IUrlClicksRepository _db;

        public StatsServices(IUrlClicksRepository db)
        {
            _db = db;
        }

        public async Task<UrlClicks?> ObtieneClickPorId(int id)
        {
            var click = await _db.ObtenerClickPorId(id);
            return click;
        }

        public async Task<IEnumerable<UrlClicks>> GetClicksByUrlId(int id)
        {
            var clicks = await _db.GetClicksByUrlIdAsync(id);
            return clicks;
        }

        public async Task<ClickStats> GetClickStats(int id)
        {
            var stats = await _db.GetClickStatsAsync(id);
            return stats;
        }

        public async Task<IEnumerable<DailyClicks>> GetDailyClicks(int id,int days = 30)
        {
            var dailyClicks = await _db.GetDailyClicksAsync(id,days);
            return dailyClicks;
        }

        public async Task<IEnumerable<CountryClicks>> GetCountryClicks(int id)
        {
            var countryClicks = await _db.GetCountryClicksAsync(id);
            return countryClicks;
        }

        public async Task<IEnumerable<DeviceStats>> GetDeviceStats(int id)
        {
            var deviceClicks = await _db.GetDeviceStatsAsync(id);
            return deviceClicks;
        }
    }
}
