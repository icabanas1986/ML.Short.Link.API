using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Interfaz
{
    public interface IUrlClicksRepository
    {
        Task<bool> IngresaClicks(UrlClicks u);
        Task<UrlClicks?> ObtenerClickPorId(int id);
        Task<IEnumerable<UrlClicks>> GetClicksByUrlIdAsync(int urlId);
        Task<ClickStats> GetClickStatsAsync(int urlId);
        Task<IEnumerable<DailyClicks>> GetDailyClicksAsync(int urlId, int days = 30);
        Task<IEnumerable<CountryClicks>> GetCountryClicksAsync(int urlId);
        Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(int urlId);
    }
}
