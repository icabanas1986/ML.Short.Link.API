using Microsoft.Data.SqlClient;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using System.Reflection.PortableExecutable;

namespace ML.Short.Link.API.Data.Service
{
    public class UrlRepositorio : IUrlRepositorio
    {
        private readonly SqlConnection _conn;
        public UrlRepositorio(SqlConnection conn)
        {
            _conn = conn;
        }

        //public async Taks<int> InsertaUrlFreeAsync(string originalUrl, string shortCode)
        //{
        //    var query = "INSERT INTO Urls (UrlOriginal, UrlCorta,fecha_creacion,clicks,activa) " +
        //        "OUTPUT INSERTED.idUrl VALUES (@OriginalUrl, @ShortCode,@fechaCreacion,@clicks,@activa)";
        //    using var command = new SqlCommand(query, _conn);
        //    command.Parameters.AddWithValue("@OriginalUrl", originalUrl);
        //    command.Parameters.AddWithValue("@ShortCode", shortCode);
        //    command.Parameters.AddWithValue("@fechaCreacion", DateTime.UtcNow);
        //    command.Parameters.AddWithValue("@clicks", 0);
        //    command.Parameters.AddWithValue("@activa", true);
        //    await _conn.OpenAsync();
        //    var id = (int)await command.ExecuteScalarAsync();
        //    await _conn.CloseAsync();
        //    return id;
        //}
        public async Task<int> InsertarUrlAsync(string originalUrl, string shortCode, int idUser)
        {
            var query = "INSERT INTO Urls (UrlOriginal, UrlCorta,fecha_creacion,clicks,activa,UserId) " +
                "OUTPUT INSERTED.idUrl VALUES (@OriginalUrl, @ShortCode,@fechaCreacion,@clicks,@activa,@userId)";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@OriginalUrl", originalUrl);
            command.Parameters.AddWithValue("@ShortCode", shortCode);
            command.Parameters.AddWithValue("@fechaCreacion", DateTime.UtcNow);
            command.Parameters.AddWithValue("@clicks", 0);
            command.Parameters.AddWithValue("@activa", true);
            command.Parameters.AddWithValue("@userId", idUser);

            await _conn.OpenAsync();
            var id = (int)await command.ExecuteScalarAsync();
            await _conn.CloseAsync();
            return id;
        }

        public async Task IncrementarClickCountAsync(string shortCode)
        {
            var query = "UPDATE Urls SET clicks = clicks + 1 WHERE UrlCorta = @ShortCode";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@ShortCode", shortCode);
            await _conn.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
        }

        public async Task<int> InsertStats(UrlClicks urlClicks)
        {
            var query = "INSERT INTO UrlClicks (UrlId,ClickedAt,IPAddress,UserAgent,Country,DeviceType) " +
                "OUTPUT INSERTED.Id VALUES (@UrlId, @ClickedAt, @IPAddress, @UserAgent, @Country, @DeviceType)";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@UrlId", urlClicks.UrlId);
            command.Parameters.AddWithValue("@ClickedAt", urlClicks.ClickedAt);
            command.Parameters.AddWithValue("@IPAddress", urlClicks.IPAddress ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UserAgent", urlClicks.UserAgent ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Country", urlClicks.Country ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DeviceType", urlClicks.DeviceType ?? (object)DBNull.Value);
            await _conn.OpenAsync();
            var id = (int)await command.ExecuteNonQueryAsync();
            await _conn.CloseAsync();
            return id;
        }
        public async Task<bool> ExisteUrlAsync(string shortCode)
        {
            var query = "SELECT COUNT(*) FROM Urls WHERE UrlCorta = @ShortCode";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@ShortCode", shortCode);
            await _conn.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();
            await _conn.CloseAsync();
            return count > 0;
        }

        public async Task<List<ShortUrl>> ObtenerUrlsPorUsuarioAsync(int userId)
        {
            var urls = new List<ShortUrl>();
            await _conn.OpenAsync();
            var query = "SELECT IdUrl, UrlOriginal, UrlCorta, fecha_creacion,clicks,UserId FROM Urls WHERE UserId = @UserId";
            using (var command = new SqlCommand(query, _conn))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        urls.Add(new ShortUrl
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("idUrl")),
                            OriginalUrl = reader.GetString(reader.GetOrdinal("UrlOriginal")),
                            ShortCode = reader.GetString(reader.GetOrdinal("UrlCorta")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                            ClickCount = reader.GetInt32(reader.GetOrdinal("clicks")),
                            IdUser = reader.GetInt32(reader.GetOrdinal("UserId"))
                        });
                    }
                }
            }
            await _conn.CloseAsync();
            return urls;
        }
        public async Task<ShortUrl> ObtenerUrlOriginalAsync(string shortCode)
        {
            var query = "SELECT idUrl,UrlOriginal FROM Urls WHERE UrlCorta = @ShortCode";
            using var command = new SqlCommand(query, _conn);
            ShortUrl? shortUrl = null;
            await _conn.OpenAsync();
            command.Parameters.AddWithValue("@ShortCode", shortCode);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                shortUrl = new ShortUrl
                {
                    Id = reader.GetInt32(reader.GetOrdinal("idUrl")),
                    OriginalUrl = reader.GetString(reader.GetOrdinal("UrlOriginal")),
                    ShortCode = shortCode
                };
            }
            
            await _conn.CloseAsync();
            return shortUrl;
        }
    }
}
