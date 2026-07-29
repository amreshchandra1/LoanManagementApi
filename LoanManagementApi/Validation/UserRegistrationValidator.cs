using FluentValidation;
using LoanManagementApi;
using LoanManagementApi.Model;
using Microsoft.EntityFrameworkCore;

public class UserRegistrationValidator : AbstractValidator<UserRegistration>
{
    private readonly EFContext _context;
    public UserRegistrationValidator(EFContext context)
    {
        _context = context;

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

    }
}
