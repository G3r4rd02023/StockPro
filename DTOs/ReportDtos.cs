using StockPro.DTOs;

namespace StockPro.DTOs
{
    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public IEnumerable<ProductReportItemDto> Products { get; set; } = [];
    }

    public class ProductReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }
        public int MinStockThreshold { get; set; }
        public decimal TotalValue { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }

    public class MovementReportDto
    {
        public int TotalEntries { get; set; }
        public int TotalExits { get; set; }
        public int TotalEntriesQuantity { get; set; }
        public int TotalExitsQuantity { get; set; }
        public IEnumerable<StockMovementDto> Movements { get; set; } = [];
    }
}
