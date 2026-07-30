using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanManagementApi.Model
{
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

      //  [Required(ErrorMessage = "Role name is required.")]
        public string RoleName { get; set; } = string.Empty;
    }
}
