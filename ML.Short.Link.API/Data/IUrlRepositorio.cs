namespace ML.Short.Link.API.Data
{
    public interface IUrlRepositorio
    {
        Task<int> InsertarUrlAsync(string originalUrl, string shortCode);
        Task<string> ObtenerUrlOriginalAsync(string shortCode);
        Task IncrementarClickCountAsync(string shortCode);
        
        Task<bool> ExisteUrlAsync(string shortCode);
    }
}
