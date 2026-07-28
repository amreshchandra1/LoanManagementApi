using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace LoanManagementApi.Model
{
    public class LoanStatusTracking
    {
        [JsonIgnore]
        public int Id { get; set; }
        public Guid LoanApplicationId { get; set; } 
        public string Status { get; set; } = string.Empty;
        public DateOnly SubmittedDate { get; set; }
    }
}
