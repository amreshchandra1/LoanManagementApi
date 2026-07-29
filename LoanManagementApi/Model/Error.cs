using Microsoft.AspNetCore.Mvc;

namespace LoanManagementApi.Model
{
    public class Error: ProblemDetails
    {
        public string InnerException { get; set; }
    }
}
