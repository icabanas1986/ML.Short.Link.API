using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Services
{
    public class UserService
    {
        private readonly IUserRepositorio _db;
        public UserService(IUserRepositorio db)
        {
            _db = db;
        }
        public async Task<int> InsertarUsuarioAsync(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Email and password hash cannot be null or empty.");
            }
            ValidarUsuarioAsync(email, passwordHash).GetAwaiter().GetResult();
            return await _db.RegistrarUsuarioAsync(email, passwordHash);
        }
        public async Task<bool> ValidarUsuarioAsync(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Email and password hash cannot be null or empty.");
            }
            return await _db.ValidarUsuarioAsync(email, passwordHash);
        }

        public async Task<User> ObtieneUsuario(string email)
        {             if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.");
            }
            var user = await _db.ObtieneUsuario(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return user;
        }
    }
}
