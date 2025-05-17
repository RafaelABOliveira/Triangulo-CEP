using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CepRepository : ICepRepository
    {
        private readonly ApplicationDbContext _context;

        public CepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CepModel> GetByCepAsync(string cep)
        {
            return await _context.CEP.FirstOrDefaultAsync(c => c.cep == cep);
        }

        public async Task AddAsync(CepModel cep)
        {
            await _context.CEP.AddAsync(cep);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}