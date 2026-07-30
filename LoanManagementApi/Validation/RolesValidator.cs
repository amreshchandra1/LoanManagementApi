using FluentValidation;
using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementApi.Validation
{
    public class RolesValidator:AbstractValidator<Roles>
    {
        private readonly EFContext _context;
        public RolesValidator(EFContext context)
        {
            _context = context;
            RuleFor(role => role.RoleName)
             .MustAsync(async (roleInstance, roleName, cancellationToken) =>
             {
                 var exists = await _context.Roles.AnyAsync(r =>
                     r.RoleName.ToLower() == roleName.ToLower() &&
                     r.Id != roleInstance.Id,
                     cancellationToken);
                 // Return true if it does NOT exist (meaning it is valid)
                 return !exists;
             })
             .WithMessage("This role name already exists.");
        }
    }
}
