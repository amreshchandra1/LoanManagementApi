using FluentValidation;
using LoanManagementApi;
using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;

public class UserRegistrationValidator : AbstractValidator<UserRegistration>
{
    private readonly EFContext _context;
    private readonly List<int> _allowedRoles;
    public UserRegistrationValidator(EFContext context)
    {
        _context = context;
        _allowedRoles = _context.Roles.Select(x => x.Id).ToList();

        // Validation rule for Email
        RuleFor(user => user.Email)
            .MustAsync(async (email, cancellation) =>
            {
                // Returns true if the email does NOT exist (validation passes)
                bool exists = await _context.UserRegistration
                    .AnyAsync(x => x.Email.ToLower() == email.ToLower() , cancellation);

                return !exists;
            })
            .WithMessage("This email address is already registered.");

        RuleFor(user => user.UserName)
            .MustAsync(async (username, cancellation) =>
            {
                // Returns true if the email does NOT exist (validation passes)
                bool exists = await _context.UserRegistration
                    .AnyAsync(x => x.UserName.ToLower() == username.ToLower(), cancellation);

                return !exists;
            })
            .WithMessage("This UserName address is already registered.");

        RuleFor(role => role.RolesId)
            .NotEmpty().WithMessage("Role Id cannot be empty.")
            .Must(roleName => _allowedRoles.Contains(roleName))
            .WithMessage((roleInstance, roleName) =>
                $"The role '{roleName}' is not allowed. Allowed roles are: {string.Join(", ", _allowedRoles)}.");

    }

}
