using StockPro.Data.Entities;
using StockPro.Data.Enums;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IStockMovementRepository _movementRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAlertService _alertService;

        public StockMovementService(
            IStockMovementRepository movementRepository,
            IProductRepository productRepository,
            IAlertService alertService)
        {
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _alertService = alertService;
        }

        public async Task<IEnumerable<StockMovementDto>> GetMovementsAsync(Guid? productId, Guid? userId, MovementType? type)
        {
            var movements = await _movementRepository.GetAllAsync(productId, userId, type);
            return movements.Select(MapToDto);
        }

        public async Task<StockMovementDto?> GetMovementByIdAsync(Guid id)
        {
            var movement = await _movementRepository.GetByIdAsync(id);
            return movement != null ? MapToDto(movement) : null;
        }

        public async Task<StockMovementDto> CreateMovementAsync(CreateStockMovementDto createMovementDto, Guid currentUserId)
        {
            // Validar que el producto existe
            var product = await _productRepository.GetByIdAsync(createMovementDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("El producto especificado no existe.");
            }

            // Guardar stock antes del movimiento
            var stockBefore = product.CurrentStock;

            // Calcular nuevo stock según el tipo de movimiento
            int stockAfter;
            if (createMovementDto.MovementType == MovementType.Entry)
            {
                stockAfter = stockBefore + createMovementDto.Quantity;
            }
            else // MovementType.Exit
            {
                if (stockBefore < createMovementDto.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente. Stock actual: {stockBefore}, Cantidad solicitada: {createMovementDto.Quantity}");
                }
                stockAfter = stockBefore - createMovementDto.Quantity;
            }

            // Crear el movimiento
            var movement = new StockMovement
            {
                ProductId = createMovementDto.ProductId,
                UserId = currentUserId,
                MovementType = createMovementDto.MovementType,
                Quantity = createMovementDto.Quantity,
                Reason = createMovementDto.Reason,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                MovementDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _movementRepository.AddAsync(movement);

            // Actualizar el stock del producto
            product.CurrentStock = stockAfter;
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);

            // Verificar y generar alertas automáticas si el stock está bajo
            await _alertService.CheckAndGenerateAlertsAsync();

            // Recargar para incluir las entidades relacionadas
            var createdMovement = await _movementRepository.GetByIdAsync(movement.Id);
            return MapToDto(createdMovement!);
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
