using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context) => _context = context;

        public async Task<List<Contract>> SearchContractsAsync(DateTime? from, DateTime? to, ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(c => c.StartDate >= from.Value);
            if (to.HasValue)
                query = query.Where(c => c.EndDate <= to.Value);
            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            return await query.OrderByDescending(c => c.StartDate).ToListAsync();
        }
    }
}
