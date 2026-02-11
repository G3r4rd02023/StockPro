using Microsoft.EntityFrameworkCore;
using StockPro.Data.Entities;
using StockPro.Data.Enums;
using StockPro.Interfaces;

namespace StockPro.Data.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly AppDbContext _context;

        public StockMovementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockMovement>> GetAllAsync(Guid? productId, Guid? userId, MovementType? type)
        {
            var query = _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.User)
                .AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(sm => sm.ProductId == productId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(sm => sm.UserId == userId.Value);
            }

            if (type.HasValue)
            {
                query = query.Where(sm => sm.MovementType == type.Value);
            }

            return await query.OrderByDescending(sm => sm.MovementDate).ToListAsync();
        }

        public async Task<StockMovement?> GetByIdAsync(Guid id)
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.User)
                .FirstOrDefaultAsync(sm => sm.Id == id);
        }

        public async Task<StockMovement> AddAsync(StockMovement movement)
        {
            await _context.StockMovements.AddAsync(movement);
            await _context.SaveChangesAsync();
            return movement;
        }

        public async Task<IEnumerable<StockMovement>> GetRecentActivityAsync(int count)
        {
            return await _context.StockMovements
                .Include(sm => sm.Product)
                .Include(sm => sm.User)
                .OrderByDescending(sm => sm.MovementDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
