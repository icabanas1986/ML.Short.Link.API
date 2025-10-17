using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils.Interface;

namespace ML.Short.Link.API.Services
{
    public class UserService
    {
        private readonly IUserRepositorio _db;
        private readonly IEmailService _emailService;
        public UserService(IUserRepositorio db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }
        public async Task<int> InsertarUsuarioAsync(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Email and password hash cannot be null or empty.");
            }
            ValidarUsuarioAsync(email, passwordHash).GetAwaiter().GetResult();
            var userId =  await _db.RegistrarUsuarioAsync(email, passwordHash);
            if(userId > 0)
            {
                var token = _emailService.GenerateConfirmationToken();
                var confirmationLink = $"https://yourdomain.com/confirm?token={token}&email={email}";
                await _emailService.SendRegistrationConfirmationAsync(email, email, confirmationLink);
            }
            return userId;
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
