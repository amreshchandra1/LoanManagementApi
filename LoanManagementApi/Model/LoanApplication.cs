using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LoanManagementApi.Model
{
    public class LoanApplication
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Customer selection is required.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Principal loan amount is required.")]
        [Range(100, 100000000, ErrorMessage = "Principal amount must be between 100 and 100,000,000.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrincipalAmount { get; set; }

        [Required(ErrorMessage = "Annual interest rate is required.")]
        [Range(0.1, 99.9, ErrorMessage = "Annual interest rate must be between 0.1% and 99.9%.")]
        public double AnnualInterestRate { get; set; }

        [Required(ErrorMessage = "Loan term in months is required.")]
        [Range(1, 480, ErrorMessage = "Term must be between 1 and 480 months (up to 40 years).")]
        public int TermInMonths { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
       // [RegularExpression("^(ApplicationSubmitted|DocumentsUploaded|DocumentsVerified|CreditCheckCompleted|UnderManagerApproval|DisbursementPending)$", ErrorMessage = "Status must be Pending, Approved, or Rejected.")]
        public string Status { get; set; } = "Pending";

        [Range(0, 10000000, ErrorMessage = "Calculated EMI must be a positive value.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CalculatedEmi { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public string UserRegistrationUserName { get; set; } = string.Empty;
    }

}
