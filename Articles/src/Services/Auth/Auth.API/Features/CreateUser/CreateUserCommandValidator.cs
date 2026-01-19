using FastEndpoints;
using FluentValidation;

namespace Auth.API.Features.CreateUser;

public class CreateUserCommandValidator : Validator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();

        RuleFor(x => x.UserRoles).NotEmpty()
            .Must((c, roles) => AreUserRoleDatesValid(roles))
            .WithMessage("Invalid roles");
        ;
    }

    private static bool AreUserRoleDatesValid(IReadOnlyList<UserRoleDto> roles)
    {
        return roles.All(x =>
            (!x.StartDate.HasValue || x.StartDate.Value.Date >= DateTime.UtcNow)
            &&
            (!x.ExpiringDate.HasValue || (x.StartDate ?? DateTime.UtcNow).Date < x.ExpiringDate.Value.Date)
        );
    }
}