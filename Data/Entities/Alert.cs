using StockPro.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPro.Data.Entities
{
    public class Alert
    {
        
        public Guid Id { get; set; }

        
        [Required]
        public Guid ProductId { get; set; }

        
        [Required]
        public AlertType AlertType { get; set; } = AlertType.LowStock;

        
        [Required]
        public string Message { get; set; } = string.Empty;

        
        [Required]
        public bool IsRead { get; set; } = false;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public DateTime? ReadAt { get; set; }

        
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
}
