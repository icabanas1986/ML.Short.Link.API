using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Interfaz
{
    public interface IUrlRepositorio
    {
        Task<int> InsertarUrlAsync(string originalUrl, string shortCode,int idUser);
        Task<ShortUrl> ObtenerUrlOriginalAsync(string shortCode);
        Task IncrementarClickCountAsync(string shortCode);
        Task<bool> ExisteUrlAsync(string shortCode);
        Task<List<ShortUrl>> ObtenerUrlsPorUsuarioAsync(int userId);

        Task<int> InsertStats(UrlClicks urlClicks);
    }
}
