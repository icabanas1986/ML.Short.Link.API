using Microsoft.Data.SqlClient;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Service
{
    public class UserRepositorio : IUserRepositorio
    {
        private readonly SqlConnection _conn;
        public UserRepositorio(SqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<int> RegistrarUsuarioAsync(string email, string password)
        {
            var query = "INSERT INTO Users (Email, PasswordHash, CreatedAt) " +
                        "OUTPUT INSERTED.Id VALUES (@Email, @Password, @FechaRegistro)";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password); // Consider hashing the password
            command.Parameters.AddWithValue("@FechaRegistro", DateTime.UtcNow);
            await _conn.OpenAsync();
            var id = (int)await command.ExecuteScalarAsync();
            await _conn.CloseAsync();
            return id;
        }
        public async Task<bool> ValidarUsuarioAsync(string email, string password)
        {
            var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PasswordHash = @Password";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password); // Consider hashing the password
            await _conn.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();
            await _conn.CloseAsync();
            return count > 0;
        }
        public async Task<User> ObtieneUsuario(string email)
        {
            var query = "SELECT Id, Email, PasswordHash, CreatedAt FROM Users WHERE Email = @Email";
            using var command = new SqlCommand(query, _conn);
            command.Parameters.AddWithValue("@Email", email);
            await _conn.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                };
            }
            await _conn.CloseAsync();
            return null; // or throw an exception if user not found
        }
    }
}
