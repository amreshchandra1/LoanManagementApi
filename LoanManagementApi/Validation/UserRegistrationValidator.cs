using FluentValidation;
using LoanManagementApi;
using LoanManagementApi.Model;
using LoanManagementApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class UserRegistrationValidator : AbstractValidator<UserRegistration>
{
    private readonly EFContext _context;
    private readonly List<int> _allowedRoles;
    private readonly List<string> _existingEmail;
    private readonly List<string> _existingUserName;
    private readonly IRoleManagement _roleManagement;
    private readonly ILoan _loan;
    public UserRegistrationValidator(EFContext context,IRoleManagement roleManagement,ILoan loan)
    {
        _context = context;
        _roleManagement = roleManagement;
        _loan = loan;

        _allowedRoles = _roleManagement.GetAllRoles().Select(x => x.Id).ToList();
        _existingEmail= _loan.GetUserRegistation().Select(x => x.Email).ToList();
        _existingUserName=_context.UserRegistration.Select(x => x.UserName).ToList();

        // Validation rule for Email
        RuleFor(user => user.Email)
            .Must(email => !_existingEmail.Contains(email))
            .WithMessage("This email address is already registered.");

        RuleFor(user => user.UserName)
            .Must(username => !_existingUserName.Contains(username))
            .WithMessage("This UserName address is already registered.");

        RuleFor(role => role.RolesId)
            .NotEmpty().WithMessage("Role Id cannot be empty.")
            .Must(roleName => _allowedRoles.Contains(roleName))
            .WithMessage((roleInstance, roleName) =>
                $"The role '{roleName}' is not allowed. Allowed roles are: {string.Join(", ", _allowedRoles)}.");

    }

}
