using System.ComponentModel.DataAnnotations;

namespace StockPro.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;
    }
}
