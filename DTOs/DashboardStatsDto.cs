namespace StockPro.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockCount { get; set; }
        public IEnumerable<StockMovementDto> RecentActivity { get; set; } = [];
    }
}
