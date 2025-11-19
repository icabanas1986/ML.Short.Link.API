using Microsoft.AspNetCore.Mvc;
using ML.Short.Link.API.Services;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly StatsServices _statsServices;
        public StatsController(StatsServices statsServices)
        {
            _statsServices = statsServices;

        }

        [HttpGet("{urlId}")]
        public async Task<IActionResult> GetStats(int urlId)
        {
            try
            {
                var stats = await _statsServices.GetClickStats(urlId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving stats", details = ex.Message });
            }
        }

        [HttpGet("{urlId}/daily")]
        public async Task<IActionResult> GetDailyClicks(int urlId, [FromQuery] int days = 30)
        {
            try
            {
                var dailyClicks = await _statsServices.GetDailyClicks(urlId, days);
                return Ok(dailyClicks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving daily clicks", details = ex.Message });
            }
        }

        [HttpGet("{urlId}/devices")]
        public async Task<IActionResult> GetDeviceStats(int urlId)
        {
            try
            {
                var deviceStats = await _statsServices.GetDeviceStats(urlId);
                return Ok(deviceStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving device stats", details = ex.Message });
            }
        }

        [HttpGet("{urlId}/full")]
        public async Task<IActionResult> GetFullStats(int urlId, [FromQuery] int days = 30)
        {
            try
            {
                var statsTask = _statsServices.GetClickStats(urlId);
                var dailyClicksTask = _statsServices.GetDailyClicks(urlId, days);
                var countryClicksTask = _statsServices.GetCountryClicks(urlId);
                var deviceStatsTask = _statsServices.GetDeviceStats(urlId);

                await Task.WhenAll(statsTask, dailyClicksTask, countryClicksTask, deviceStatsTask);

                var result = new
                {
                    Stats = statsTask.Result,
                    DailyClicks = dailyClicksTask.Result,
                    CountryClicks = countryClicksTask.Result,
                    DeviceStats = deviceStatsTask.Result
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving full stats", details = ex.Message });
            }
        }
    }
}
