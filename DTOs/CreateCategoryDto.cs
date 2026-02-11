using System.ComponentModel.DataAnnotations;

namespace StockPro.DTOs
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es requerido")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "El color debe estar en formato hexadecimal (#RRGGBB)")]
        public string ColorHex { get; set; } = "#6B7280";
    }
}
