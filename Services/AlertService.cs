using StockPro.Data.Entities;
using StockPro.Data.Enums;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IProductRepository _productRepository;

        public AlertService(IAlertRepository alertRepository, IProductRepository productRepository)
        {
            _alertRepository = alertRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync(bool? unreadOnly)
        {
            var alerts = await _alertRepository.GetAllAsync(unreadOnly);
            return alerts.Select(MapToDto);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _alertRepository.GetUnreadCountAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
            {
                throw new KeyNotFoundException($"Alerta con ID {id} no encontrada.");
            }

            alert.IsRead = true;
            alert.ReadAt = DateTime.UtcNow;
            await _alertRepository.UpdateAsync(alert);
        }

        public async Task MarkAllAsReadAsync()
        {
            var unreadAlerts = await _alertRepository.GetAllAsync(true);
            foreach (var alert in unreadAlerts)
            {
                alert.IsRead = true;
                alert.ReadAt = DateTime.UtcNow;
                await _alertRepository.UpdateAsync(alert);
            }
        }

        public async Task CheckAndGenerateAlertsAsync()
        {
            // Obtener todos los productos con bajo stock
            var lowStockProducts = await _productRepository.GetAllAsync(null, null, true);

            foreach (var product in lowStockProducts)
            {
                // No crear alerta duplicada si ya existe una activa no leída para este producto
                if (await _alertRepository.ExistsActiveAlertForProductAsync(product.Id))
                {
                    continue;
                }

                var alertType = product.CurrentStock == 0 ? AlertType.OutOfStock : AlertType.LowStock;
                var message = product.CurrentStock == 0
                    ? $"¡AGOTADO! El producto '{product.Name}' (SKU: {product.SKU}) se ha agotado."
                    : $"Stock bajo: El producto '{product.Name}' (SKU: {product.SKU}) tiene {product.CurrentStock} unidades (umbral: {product.MinStockThreshold}).";

                var alert = new Alert
                {
                    ProductId = product.Id,
                    AlertType = alertType,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _alertRepository.AddAsync(alert);
            }
        }

        private AlertDto MapToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                ProductId = alert.ProductId,
                ProductName = alert.Product?.Name ?? "Producto desconocido",
                AlertType = alert.AlertType,
                Message = alert.Message,
                IsRead = alert.IsRead,
                CreatedAt = alert.CreatedAt,
                ReadAt = alert.ReadAt
            };
        }
    }
}
