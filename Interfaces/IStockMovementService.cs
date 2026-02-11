using StockPro.Data.Enums;
using StockPro.DTOs;

namespace StockPro.Interfaces
{
    public interface IStockMovementService
    {
        Task<IEnumerable<StockMovementDto>> GetMovementsAsync(Guid? productId, Guid? userId, MovementType? type);
        Task<StockMovementDto?> GetMovementByIdAsync(Guid id);
        Task<StockMovementDto> CreateMovementAsync(CreateStockMovementDto createMovementDto, Guid currentUserId);
    }
}
