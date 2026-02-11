using Microsoft.EntityFrameworkCore;
using StockPro.Data.Entities;
using StockPro.Interfaces;

namespace StockPro.Data.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _context;

        public AlertRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alert>> GetAllAsync(bool? unreadOnly)
        {
            var query = _context.Alerts
                .Include(a => a.Product)
                .AsQueryable();

            if (unreadOnly.HasValue && unreadOnly.Value)
            {
                query = query.Where(a => !a.IsRead);
            }

            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<Alert?> GetByIdAsync(Guid id)
        {
            return await _context.Alerts
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Alert> AddAsync(Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task UpdateAsync(Alert alert)
        {
            _context.Alerts.Update(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.Alerts.CountAsync(a => !a.IsRead);
        }

        public async Task<bool> ExistsActiveAlertForProductAsync(Guid productId)
        {
            return await _context.Alerts.AnyAsync(a => a.ProductId == productId && !a.IsRead);
        }
    }
}
