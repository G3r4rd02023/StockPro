using StockPro.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockPro.Data.Entities
{
    public class User
    {
        
        public Guid Id { get; set; }

       
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

       
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        
        [Required]
        public UserRole Role { get; set; } = UserRole.Employee;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        
        public virtual ICollection<StockMovement> StockMovements { get; set; } = [];
    }
}
