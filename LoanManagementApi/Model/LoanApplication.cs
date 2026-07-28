using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LoanManagementApi.Model
{
    public class LoanApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public double AnnualInterestRate { get; set; } // e.g. 7.5 for 7.5%
        public int TermInMonths { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public decimal CalculatedEmi { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ValidateNever]
        public string UserRegistrationUserName { get; set; }
    }

}
