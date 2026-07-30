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
            var lstStatus = Enum.GetNames<LoanStatus>().ToList();
            //Rule for status that status cant be updated to previous value
            RuleFor(x => x.Status)
            
             .Must( (model, incomingStatus) =>
             {
                 // 1. Fetch the CURRENT status of this loan from the database
                 //var currentApplication =  _context.LoanApplications
                 //    .AsNoTracking()
                 //    .Where(l => l.Id == model.Id)
                 //    .Select(l => new { l.Status })
                 //    .FirstOrDefault();
                 var currentApplication = new LoanApplication() { Status = "DocumentsUploaded" };
                 // If the record doesn't exist yet, it's a new insertion; allow it
                 if (currentApplication == null) return true;

                 // 2. Cast enums to integers to compare their numeric positions
                 int? currentStatusValue = _helper.TryGetLoanStatusIntValue(currentApplication.Status);
                 int? incomingStatusValue = _helper.TryGetLoanStatusIntValue(incomingStatus);

                 // 3. VALID LOGIC: Incoming must be greater than current status
                 return incomingStatusValue > currentStatusValue;
             })
             .WithMessage((model, incomingStatus) =>
                 $"Cannot update status to '{incomingStatus}'. Moving backwards or re-submitting the same status is not allowed.");
        }
    }
}
