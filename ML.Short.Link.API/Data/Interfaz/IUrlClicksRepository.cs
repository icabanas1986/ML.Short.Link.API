using ML.Short.Link.API.Models;

namespace ML.Short.Link.API.Data.Interfaz
{
    public interface IUrlClicksRepository
    {
        Task<bool> IngresaClicks(UrlClicks u);
    }
}
