using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPro.Data.Entities
{
    public class Product
    {
        
        public Guid Id { get; set; }

        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;

        
        [Required]
        public Guid CategoryId { get; set; }

        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Price { get; set; }

        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int CurrentStock { get; set; } = 0;

        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El umbral mínimo no puede ser negativo")]
        public int MinStockThreshold { get; set; } = 10;

        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        
        [MaxLength(500)]
        public string? QrCode { get; set; }

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        
        public virtual ICollection<StockMovement> StockMovements { get; set; } = [];

        
        public virtual ICollection<Alert> Alerts { get; set; } = [];

        
        [NotMapped]
        public bool IsLowStock => CurrentStock <= MinStockThreshold;

        
        [NotMapped]
        public bool IsOutOfStock => CurrentStock == 0;

        
        [NotMapped]
        public decimal TotalValue => CurrentStock * Price;
    }
}
