using Auth.Domain.Users.Enums;

namespace Auth.Domain.Users;

public interface IUserCreationInfo
{
    string Email { get; }
    string Name { get; }
    Gender Gender { get; }
    Honorific? Honorific { get; }
    string? PhoneNumber { get; }
    string? PictureUrl { get; }
    string? CompanyName { get; }
    string? Position { get; }
    string? Affiliation { get; }

    IReadOnlyList<IUserRole> UserRoles { get; }
}