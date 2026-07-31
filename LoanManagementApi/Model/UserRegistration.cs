using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanManagementApi.Model
{
    public class UserRegistration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Foreign Key Relationship Setup
        [Required(ErrorMessage = "Role assignment is required.")]
        //[AllowedValues(1,2,3, ErrorMessage = "Role must be either 'Admin' or 'Loan Officer' or 'Customer'.Give 1 for Admin,2 for Loan Officer,3 for Customer")]
       // [AllowedValues("Admin", "Loan Officer", "Customer", ErrorMessage = "Role must be either 'Admin' or 'Loan Officer' or 'Customer'")]
        [NotMapped]
        public string RoleName { get; set; } = string.Empty;
        [ValidateNever]
        public int RolesId { get; set; }

        [ForeignKey(nameof(RolesId))]
        [ValidateNever] // Prevents API model binding errors when payload doesn't send full role object
        [JsonIgnore]
        public Roles Roles { get; set; } = null!;
    }
}
