using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementApi.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        [StringLength(255, ErrorMessage = "Password hash cannot exceed 255 characters.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "User role is required.")]
        [StringLength(30, ErrorMessage = "Role name cannot exceed 30 characters.")]
        [RegularExpression("^(Admin|LoanOfficer|Customer)$", ErrorMessage = "Role must be Admin, LoanOfficer, or Customer.")]
        public string Role { get; set; } = string.Empty;
    }
}
