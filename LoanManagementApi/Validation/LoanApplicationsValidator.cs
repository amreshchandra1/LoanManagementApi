using FluentValidation;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

namespace LoanManagementApi.Validation
{
    public class LoanApplicationsValidator:AbstractValidator<LoanApplication>
    {
        private List<string> lstStatus;
        private readonly EFContext _context;
        private readonly IHelper _helper;
        public LoanApplicationsValidator(EFContext context,IHelper helper)
        {
            _context = context;
            _helper = helper;
           // var lstStatus = Enum.GetNames<LoanStatus>().ToList();
            //Rule for status that status cant be updated to previous value
            RuleFor(x => x.Status)
             .NotEmpty().WithMessage("Status cannot be empty.")
             .Must(statusString =>
             {
                 // 1. Attempt to parse the incoming string into our LoanStatus enum (ignoring case)
                 // 2. Enum.TryParse natively ensures the value matches an explicitly named item
                 return Enum.TryParse<LoanStatus>(statusString, true, out _);
             })
             .WithMessage((instance, statusString) =>
                 $"The status '{statusString}' is invalid. Please provide a valid loan status option.");
        }
    }
}
