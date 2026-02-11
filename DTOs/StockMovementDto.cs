using StockPro.Data.Enums;

namespace StockPro.DTOs
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public MovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
