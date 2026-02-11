using StockPro.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockPro.DTOs
{
    public class CreateStockMovementDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es requerido")]
        public MovementType MovementType { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "La razón es requerida")]
        [MaxLength(500, ErrorMessage = "La razón no puede exceder los 500 caracteres")]
        public string Reason { get; set; } = string.Empty;
    }
}
