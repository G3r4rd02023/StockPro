using System.ComponentModel.DataAnnotations;

namespace StockPro.Data.Entities
{
    public class Category
    {
        
        public Guid Id { get; set; }

        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

       
        [Required]
        [MaxLength(7)]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$",
            ErrorMessage = "El color debe estar en formato hexadecimal (#RRGGBB)")]
        public string ColorHex { get; set; } = "#6B7280";

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
