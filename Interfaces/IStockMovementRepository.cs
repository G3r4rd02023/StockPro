using StockPro.Data.Entities;
using StockPro.Data.Enums;

namespace StockPro.Interfaces
{
    public interface IStockMovementRepository
    {
        Task<IEnumerable<StockMovement>> GetAllAsync(Guid? productId, Guid? userId, MovementType? type);
        Task<StockMovement?> GetByIdAsync(Guid id);
        Task<StockMovement> AddAsync(StockMovement movement);
        Task<IEnumerable<StockMovement>> GetRecentActivityAsync(int count);
    }
}
