using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICepService
    {
        Task<CepModel> BuscarCepAsync(string cep);
    }
}
