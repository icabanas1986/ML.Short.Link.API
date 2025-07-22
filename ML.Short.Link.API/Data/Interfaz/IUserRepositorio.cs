using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Interfaz
{
    public interface IUserRepositorio
    {
        Task<int> RegistrarUsuarioAsync(string email, string password);

        Task<bool> ValidarUsuarioAsync(string email, string password);

        Task<User> ObtieneUsuario(string email);
    }
}
