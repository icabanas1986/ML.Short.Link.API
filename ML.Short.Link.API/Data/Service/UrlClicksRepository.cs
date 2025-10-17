using Microsoft.Data.SqlClient;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils;

namespace ML.Short.Link.API.Data.Service
{
    public class UrlClicksRepository:IUrlClicksRepository
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

                var query = "INSERT INTO UrlClicks (UrlId,ClickedAt,IPAddress,UserAgent,Country,DeviceType,City,CountryCode"+
                    ",Region,Latitude,Longitude,Timezone,PostalCode,Browser,Platform,IsMobile,IsBot,ISP,Organization)" +
                    "OUTPUT INSERTED.Id VALUES(@UrlId,@ClicketAt,@IPAddress,@UserAgent,@Country,@DeviceType,@City,@CountryCode"+
                    ",@Region,@Latitude,@Longitude,@Timezone,@PostalCode,@Browser,@Platform,@IsMobile,@IsBot,@ISP,@Organization)";
                using var command = new SqlCommand(query, _conn);
                command.Parameters.AddWithValue("@UrlId", u.UrlId);
                command.Parameters.AddWithValue("@ClicketAt", u.ClicketAt);
                command.Parameters.AddWithValue("@IPAddress", u.IPAddress);
                command.Parameters.AddWithValue("@UserAgent", u.UserAgent);
                command.Parameters.AddWithValue("@Country", u.Country);
                command.Parameters.AddWithValue("@DeviceType",u.DeviceType);
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
                SimpleLogger.LogError("UrlClicksRepository", ex,"Error al ingresar click en la base de datos");
                var msg = ex.Message;
                return false;
            }
        }

    }
}
