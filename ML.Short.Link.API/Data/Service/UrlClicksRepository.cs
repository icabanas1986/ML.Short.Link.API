using Microsoft.Data.SqlClient;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils;
using System.Data;

namespace ML.Short.Link.API.Data.Service
{
    public class UrlClicksRepository : IUrlClicksRepository
    {
        private readonly SqlConnection _conn;

        public UrlClicksRepository(SqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<bool> IngresaClicks(UrlClicks u)
        {
            try
            {
                SimpleLogger.LogInfo("UrlClicksRepository", "Ingresando click en la base de datos");

                var query = "INSERT INTO UrlClicks (UrlId,ClickedAt,IPAddress,UserAgent,Country,DeviceType,City,CountryCode" +
                    ",Region,Latitude,Longitude,Timezone,PostalCode,Browser,Platform,IsMobile,IsBot,ISP,Organization)" +
                    "OUTPUT INSERTED.Id VALUES(@UrlId,@ClicketAt,@IPAddress,@UserAgent,@Country,@DeviceType,@City,@CountryCode" +
                    ",@Region,@Latitude,@Longitude,@Timezone,@PostalCode,@Browser,@Platform,@IsMobile,@IsBot,@ISP,@Organization)";
                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", u.UrlId);
                command.Parameters.AddWithValue("@ClicketAt", u.ClickedAt);
                command.Parameters.AddWithValue("@IPAddress", u.IPAddress);
                command.Parameters.AddWithValue("@UserAgent", u.UserAgent);
                command.Parameters.AddWithValue("@Country", u.Country);
                command.Parameters.AddWithValue("@DeviceType", u.DeviceType);
                command.Parameters.AddWithValue("@City", u.City);
                command.Parameters.AddWithValue("@CountryCode", u.CountryCode);
                command.Parameters.AddWithValue("@Region", u.Region);
                command.Parameters.AddWithValue("@Latitude", (object?)u.Latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Longitude", (object?)u.Longitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@Timezone", u.Timezone);
                command.Parameters.AddWithValue("@PostalCode", u.PostalCode);
                command.Parameters.AddWithValue("@Browser", u.Browser);
                command.Parameters.AddWithValue("@Platform", u.Platform);
                command.Parameters.AddWithValue("@IsMobile", u.IsMobile);
                command.Parameters.AddWithValue("@IsBot", u.IsBot);
                command.Parameters.AddWithValue("@ISP", u.ISP);
                command.Parameters.AddWithValue("@Organization", u.Organization);

                SimpleLogger.LogInfo("UrlClicksRepository", "Ejecutando comando SQL para insertar click");
                await _conn.OpenAsync();
                var id = (int)await command.ExecuteScalarAsync();
                await _conn.CloseAsync();
                return Convert.ToBoolean(id);
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al ingresar click en la base de datos");
                var msg = ex.Message;
                return false;
            }
        }

        public async Task<UrlClicks?> ObtenerClickPorId(int id)
        {
            try
            {
                var query = "SELECT * FROM UrlClicks WHERE Id = @Id";
                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@Id", id);
                await _conn.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var click = new UrlClicks
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UrlId = reader.GetInt32(reader.GetOrdinal("UrlId")),
                        ClickedAt = reader.GetDateTime(reader.GetOrdinal("ClickedAt")),
                        IPAddress = reader.GetString(reader.GetOrdinal("IPAddress")),
                        UserAgent = reader.GetString(reader.GetOrdinal("UserAgent")),
                        Country = reader.GetString(reader.GetOrdinal("Country")),
                        DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        CountryCode = reader.GetString(reader.GetOrdinal("CountryCode")),
                        Region = reader.GetString(reader.GetOrdinal("Region")),
                        Latitude = reader.IsDBNull(reader.GetOrdinal("Latitude")) ? null : reader.GetDouble(reader.GetOrdinal("Latitude")),
                        Longitude = reader.IsDBNull(reader.GetOrdinal("Longitude")) ? null : reader.GetDouble(reader.GetOrdinal("Longitude")),
                        Timezone = reader.GetString(reader.GetOrdinal("Timezone")),
                        PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                        Browser = reader.GetString(reader.GetOrdinal("Browser")),
                        Platform = reader.GetString(reader.GetOrdinal("Platform")),
                        IsMobile = reader.GetBoolean(reader.GetOrdinal("IsMobile")),
                        IsBot = reader.GetBoolean(reader.GetOrdinal("IsBot")),
                        ISP = reader.GetString(reader.GetOrdinal("ISP")),
                        Organization = reader.GetString(reader.GetOrdinal("Organization"))
                    };
                    await _conn.CloseAsync();
                    return click;
                }
                await _conn.CloseAsync();
                return null;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener click por Id");
                return null;
            }
        }

        public async Task<IEnumerable<UrlClicks>> GetClicksByUrlIdAsync(int urlId)
        {
            var clicks = new List<UrlClicks>();
            try
            {
                await _conn.OpenAsync();

                const string query = @"
                SELECT Id, UrlId, ClickedAt, IPAddress, UserAgent, 
                       Country, CountryCode, Region, City, Latitude, Longitude, PostalCode,
                       DeviceType, Browser, Platform, IsMobile, IsBot
                FROM UrlClicks 
                WHERE UrlId = @UrlId 
                ORDER BY ClickedAt DESC";

                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", urlId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    clicks.Add(new UrlClicks
                    {
                        Id = reader.GetInt32("Id"),
                        UrlId = reader.GetInt32("UrlId"),
                        ClickedAt = reader.GetDateTime("ClickedAt"),
                        IPAddress = reader.GetString("IPAddress"),
                        UserAgent = reader.GetString("UserAgent"),
                        Country = reader.IsDBNull("Country") ? null : reader.GetString("Country"),
                        CountryCode = reader.IsDBNull("CountryCode") ? null : reader.GetString("CountryCode"),
                        Region = reader.IsDBNull("Region") ? null : reader.GetString("Region"),
                        City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                        Latitude = reader.IsDBNull("Latitude") ? null : reader.GetDouble("Latitude"),
                        Longitude = reader.IsDBNull("Longitude") ? null : reader.GetDouble("Longitude"),
                        PostalCode = reader.IsDBNull("PostalCode") ? null : reader.GetString("PostalCode"),
                        DeviceType = reader.IsDBNull("DeviceType") ? null : reader.GetString("DeviceType"),
                        Browser = reader.IsDBNull("Browser") ? null : reader.GetString("Browser"),
                        Platform = reader.IsDBNull("Platform") ? null : reader.GetString("Platform"),
                        IsMobile = reader.GetBoolean("IsMobile"),
                        IsBot = reader.GetBoolean("IsBot")
                    });
                }

                return clicks;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener clicks por UrlId");
                return clicks;
            }
            
        }

        public async Task<ClickStats> GetClickStatsAsync(int urlId)
        {
            try
            {
                await _conn.OpenAsync();

                const string query = @"
                -- Total clicks
                SELECT COUNT(*) as TotalClicks 
                FROM UrlClicks 
                WHERE UrlId = @UrlId;

                -- Unique visitors
                SELECT COUNT(DISTINCT IPAddress) as UniqueVisitors 
                FROM UrlClicks 
                WHERE UrlId = @UrlId;

                -- Top country
                SELECT TOP 1 Country as TopCountry
                FROM UrlClicks 
                WHERE UrlId = @UrlId AND Country IS NOT NULL AND Country != 'Unknown'
                GROUP BY Country 
                ORDER BY COUNT(*) DESC;";

                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", urlId);

                var stats = new ClickStats();

                using var reader = await command.ExecuteReaderAsync();

                // Total clicks
                if (await reader.ReadAsync())
                {
                    stats.TotalClicks = reader.GetInt32("TotalClicks");
                }

                // Unique visitors
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    stats.UniqueVisitors = reader.GetInt32("UniqueVisitors");
                }

                // Top country
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    stats.TopCountry = reader.IsDBNull("TopCountry") ? "No data" : reader.GetString("TopCountry");
                }

                await _conn.CloseAsync();
                return stats;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener estadísticas de clicks");
                return new ClickStats();
            }
            
        }

        public async Task<IEnumerable<DailyClicks>> GetDailyClicksAsync(int urlId, int days = 30)
        {
            var dailyClicks = new List<DailyClicks>();
            try
            {
                await _conn.OpenAsync();

                const string query = @"
                SELECT 
                    CAST(ClickedAt AS DATE) as Date,
                    COUNT(*) as Clicks
                FROM UrlClicks 
                WHERE UrlId = @UrlId 
                    AND ClickedAt >= DATEADD(DAY, -@Days, GETUTCDATE())
                GROUP BY CAST(ClickedAt AS DATE)
                ORDER BY Date";

                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", urlId);
                command.Parameters.AddWithValue("@Days", days);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    dailyClicks.Add(new DailyClicks
                    {
                        Date = reader.GetDateTime("Date"),
                        Clicks = reader.GetInt32("Clicks")
                    });
                }

                // Rellenar días sin clicks
                var result = new List<DailyClicks>();
                for (var i = days - 1; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddDays(-i).Date;
                    var clicks = dailyClicks.FirstOrDefault(x => x.Date == date)?.Clicks ?? 0;
                    result.Add(new DailyClicks
                    {
                        Date = date,
                        Clicks = clicks
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener clicks diarios");
                return dailyClicks;
            }
            
        }

        public async Task<IEnumerable<CountryClicks>> GetCountryClicksAsync(int urlId)
        {
            var countryClicks = new List<CountryClicks>();
            try
            {
                await _conn.OpenAsync();

                const string query = @"
                SELECT 
                    ISNULL(Country, 'Unknown') as Country,
                    COUNT(*) as Clicks
                FROM UrlClicks 
                WHERE UrlId = @UrlId 
                GROUP BY Country
                ORDER BY Clicks DESC
                OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", urlId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    countryClicks.Add(new CountryClicks
                    {
                        Country = reader.GetString("Country"),
                        Clicks = reader.GetInt32("Clicks")
                    });
                }

                return countryClicks;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener clicks por país");
                return countryClicks;
            }
            
        }

        public async Task<IEnumerable<DeviceStats>> GetDeviceStatsAsync(int urlId)
        {
            var deviceStats = new List<DeviceStats>();

            try
            {
                await _conn.OpenAsync();

                const string query = @"
                SELECT 
                    ISNULL(DeviceType, 'Unknown') as DeviceType,
                    COUNT(*) as Clicks
                FROM UrlClicks 
                WHERE UrlId = @UrlId 
                GROUP BY DeviceType
                ORDER BY Clicks DESC";

                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", urlId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    deviceStats.Add(new DeviceStats
                    {
                        DeviceType = reader.GetString("DeviceType"),
                        Clicks = reader.GetInt32("Clicks")
                    });
                }

                return deviceStats;
            }
            catch (Exception ex)
            {
                SimpleLogger.LogError("UrlClicksRepository", ex, "Error al obtener estadísticas por dispositivo");
                return deviceStats;
            }
            
        }
    }
}
