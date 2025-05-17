using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICepRepository
    {
        Task<CepModel> GetByCepAsync(string cep);
        Task AddAsync(CepModel cep);
        Task SaveChangesAsync();
    }
}