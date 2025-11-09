namespace OpinionsAnalytics.Persistence.Repositories.Db
{
    using OpinionsAnalytics.Application.Repositories;
    using OpinionsAnalytics.Domain.Entities.Db;
    using OpinionsAnalytics.Persistence.Context;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ResenasRepository : IResenasRepository
    {
        private readonly ResenasContext _context;

        public ResenasRepository(ResenasContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resenas>> GetResenasDataAsync()
        {
            return await _context.Resenas.Take(1000).ToListAsync();
        }
    }
}