using System.Text;
using StockPro.Data.Enums;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class ReportService : IReportService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockMovementRepository _movementRepository;

        public ReportService(
            IProductRepository productRepository,
            IStockMovementRepository movementRepository)
        {
            _productRepository = productRepository;
            _movementRepository = movementRepository;
        }

        public async Task<InventoryReportDto> GetInventoryReportAsync()
        {
            var products = await _productRepository.GetAllAsync(null, null, null);
            var productList = products.ToList();

            return new InventoryReportDto
            {
                TotalProducts = productList.Count,
                TotalStockValue = productList.Sum(p => p.CurrentStock * p.Price),
                LowStockCount = productList.Count(p => p.CurrentStock <= p.MinStockThreshold && p.CurrentStock > 0),
                OutOfStockCount = productList.Count(p => p.CurrentStock == 0),
                Products = productList.Select(p => new ProductReportItemDto
                {
                    Name = p.Name,
                    SKU = p.SKU,
                    CategoryName = p.Category?.Name ?? "Sin categoría",
                    Price = p.Price,
                    CurrentStock = p.CurrentStock,
                    MinStockThreshold = p.MinStockThreshold,
                    TotalValue = p.CurrentStock * p.Price,
                    StockStatus = p.CurrentStock == 0 ? "Agotado"
                        : p.CurrentStock <= p.MinStockThreshold ? "Bajo Stock"
                        : "Normal"
                })
            };
        }

        public async Task<MovementReportDto> GetMovementReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var allMovements = await _movementRepository.GetAllAsync(null, null, null);
            var movementList = allMovements.ToList();

            // Filtrar por rango de fechas si se proporcionan
            if (startDate.HasValue)
                movementList = movementList.Where(m => m.MovementDate >= startDate.Value).ToList();
            if (endDate.HasValue)
                movementList = movementList.Where(m => m.MovementDate <= endDate.Value).ToList();

            var entries = movementList.Where(m => m.MovementType == MovementType.Entry).ToList();
            var exits = movementList.Where(m => m.MovementType == MovementType.Exit).ToList();

            return new MovementReportDto
            {
                TotalEntries = entries.Count,
                TotalExits = exits.Count,
                TotalEntriesQuantity = entries.Sum(m => m.Quantity),
                TotalExitsQuantity = exits.Sum(m => m.Quantity),
                Movements = movementList.OrderByDescending(m => m.MovementDate).Select(m => new StockMovementDto
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    ProductName = m.Product?.Name ?? "Producto desconocido",
                    UserId = m.UserId,
                    UserName = m.User?.FullName ?? "Usuario desconocido",
                    MovementType = m.MovementType,
                    Quantity = m.Quantity,
                    Reason = m.Reason,
                    StockBefore = m.StockBefore,
                    StockAfter = m.StockAfter,
                    MovementDate = m.MovementDate
                })
            };
        }

        public async Task<byte[]> ExportInventoryToCsvAsync()
        {
            var report = await GetInventoryReportAsync();
            var sb = new StringBuilder();

            sb.AppendLine("Nombre,SKU,Categoría,Precio,Stock Actual,Umbral Mínimo,Valor Total,Estado");
            foreach (var p in report.Products)
            {
                sb.AppendLine($"\"{p.Name}\",\"{p.SKU}\",\"{p.CategoryName}\",{p.Price},{p.CurrentStock},{p.MinStockThreshold},{p.TotalValue},\"{p.StockStatus}\"");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        public async Task<byte[]> ExportMovementsToCsvAsync(DateTime? startDate, DateTime? endDate)
        {
            var report = await GetMovementReportAsync(startDate, endDate);
            var sb = new StringBuilder();

            sb.AppendLine("Fecha,Producto,Tipo,Cantidad,Razón,Stock Antes,Stock Después,Usuario");
            foreach (var m in report.Movements)
            {
                var tipo = m.MovementType == MovementType.Entry ? "Entrada" : "Salida";
                sb.AppendLine($"{m.MovementDate:yyyy-MM-dd HH:mm},\"{m.ProductName}\",\"{tipo}\",{m.Quantity},\"{m.Reason}\",{m.StockBefore},{m.StockAfter},\"{m.UserName}\"");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }
    }
}
