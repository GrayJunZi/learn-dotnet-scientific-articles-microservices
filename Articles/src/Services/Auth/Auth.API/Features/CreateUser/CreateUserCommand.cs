using Articles.Abstractions.Enums;
using Auth.Domain.Users;
using Auth.Domain.Users.Enums;

namespace Auth.API.Features.CreateUser;

public class CreateUserCommand : IUserCreationInfo
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required Gender Gender { get; set; }
    public Honorific? Honorific { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PictureUrl { get; set; }
    public string? CompanyName { get; set; }
    public string? Position { get; set; }
    public string? Affiliation { get; set; }

    public required IReadOnlyList<UserRoleDto> UserRoles { get; init; } = [];

    IReadOnlyList<IUserRole> IUserCreationInfo.UserRoles => UserRoles;
}

public record UserRoleDto(UserRoleType Type, DateTime? StartDate, DateTime? ExpiringDate) : IUserRole;

public record CreateUserResponse(string Email, int UserId, string Token);