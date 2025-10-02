using Microsoft.Data.SqlClient;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Service
{
    public class UrlRepositorio : IUrlRepositorio
    {
        private readonly SqlConnection _conn;
        public UrlRepositorio(SqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<int> InsertarUrlAsync(string originalUrl, string shortCode, int idUser)
        {
            try
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
            catch (Exception ex)
            {
                var msg = ex.Message;
                return 0;
            }
        }

        public async Task<string> ObtenerUrlOriginalAsync(string shortCode)
        {
            var query = "SELECT UrlOriginal FROM Urls WHERE UrlCorta = @ShortCode";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@ShortCode", shortCode);
            await _conn.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            await _conn.CloseAsync();
            return result as string;
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

        public async Task<List<ShortUrl>> ObtieneUrlsPorUsuario(int idUser)
        {
            var urls = new List<ShortUrl>();
            var query = "SELECT idUrl, UrlOriginal, UrlCorta, fecha_creacion, clicks, activa, UserId FROM Urls WHERE UserId = @UserId";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@UserId", idUser);
            await _conn.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                urls.Add(new ShortUrl
                {
                    Id = reader.GetInt32(0),
                    OriginalUrl = reader.GetString(1),
                    ShortCode = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    ClickCount = reader.GetInt32(4),
                    Activa = reader.GetBoolean(5),
                    IdUser = reader.GetInt32(6)
                });
            }
            await _conn.CloseAsync();
            return urls;
        }
    }
}