using StockPro.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPro.Data.Entities
{
    public class StockMovement
    {
        
        public Guid Id { get; set; }

        
        [Required]
        public Guid ProductId { get; set; }

        
        [Required]
        public Guid UserId { get; set; }

        
        [Required]
        public MovementType MovementType { get; set; }

        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockBefore { get; set; }

        
        [Required]
        [Range(0, int.MaxValue)]
        public int StockAfter { get; set; }

        
        [Required]
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
