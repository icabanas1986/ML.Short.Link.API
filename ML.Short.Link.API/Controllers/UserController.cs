using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ML.Short.Link.API.JWT;
using ML.Short.Link.API.Services;
using ML.Short.Link.API.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _user;
        private readonly PasswordHasher _passwordHasher;
        private readonly jwtServices _jwt;

        public UserController(UserService user,PasswordHasher passwordHasher, IConfiguration config,jwtServices jwt)
        {
            _user = user;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(string email, string password)
        {
            // Aquí iría la lógica para registrar un usuario
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Email y contraseña son obligatorios.");
            }
            password = _passwordHasher.HashPassword(password);

            var idUser = await _user.InsertarUsuarioAsync(email, password); // Aquí deberías manejar el resultado y posibles excepciones
            // Por ejemplo, guardar el usuario en la base de datos
            return Ok(new { Message = "Usuario registrado correctamente" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            // Aquí iría la lógica para iniciar sesión
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Email y contraseña son obligatorios.");
            }
            password = _passwordHasher.HashPassword(password);

            var isValidUser = await _user.ValidarUsuarioAsync(email, password); // Aquí deberías manejar el resultado y posibles excepciones
            if (!isValidUser)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var u = await _user.ObtieneUsuario(email);

            var token = _jwt.generaToken(email); // Genera el token JWT

            return Ok(new { jwt = token ,id= u.Id});
        }

        [HttpPost("generandoToken")]
        public async Task<IActionResult> GetUserToken(string email)
        {
            // Aquí iría la lógica para obtener el token del usuario
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email es obligatorio.");
            }
            var user = await _user.ObtieneUsuario(email); // Aquí deberías manejar el resultado y posibles excepciones
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }
            var token = _jwt.generaToken(user.Email); // Genera el token JWT
            return Ok(new { Token = token });
        }
    }
}
