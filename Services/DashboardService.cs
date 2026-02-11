using StockPro.Data.Entities;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockMovementRepository _stockMovementRepository;

        public DashboardService(
            IProductRepository productRepository,
            IStockMovementRepository stockMovementRepository)
        {
            _productRepository = productRepository;
            _stockMovementRepository = stockMovementRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalProducts = await _productRepository.CountAsync();
            var totalStockValue = await _productRepository.GetTotalStockValueAsync();
            var lowStockCount = await _productRepository.GetLowStockCountAsync();

            // Obtener últimos 5 movimientos
            var recentMovements = await _stockMovementRepository.GetRecentActivityAsync(5);

            return new DashboardStatsDto
            {
                TotalProducts = totalProducts,
                TotalStockValue = totalStockValue,
                LowStockCount = lowStockCount,
                RecentActivity = recentMovements.Select(MapToDto)
            };
        }

        private StockMovementDto MapToDto(StockMovement movement)
        {
            return new StockMovementDto
            {
                Id = movement.Id,
                ProductId = movement.ProductId,
                ProductName = movement.Product?.Name ?? "Producto desconocido",
                UserId = movement.UserId,
                UserName = movement.User?.FullName ?? "Usuario desconocido",
                MovementType = movement.MovementType,
                Quantity = movement.Quantity,
                Reason = movement.Reason,
                StockBefore = movement.StockBefore,
                StockAfter = movement.StockAfter,
                MovementDate = movement.MovementDate
            };
        }
    }
}
