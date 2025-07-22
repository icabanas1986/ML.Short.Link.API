namespace ML.Short.Link.API.Data.Interfaz
{
    public interface IUrlRepositorio
    {
        Task<int> InsertarUrlAsync(string originalUrl, string shortCode,int idUser);
        Task<string> ObtenerUrlOriginalAsync(string shortCode);
        Task IncrementarClickCountAsync(string shortCode);
        
        Task<bool> ExisteUrlAsync(string shortCode);
    }
}
