using System.ComponentModel.DataAnnotations;

namespace StockPro.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El SKU es requerido")]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es requerida")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int CurrentStock { get; set; }

        [Required(ErrorMessage = "El umbral de stock mínimo es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El umbral debe ser al menos 1")]
        public int MinStockThreshold { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
