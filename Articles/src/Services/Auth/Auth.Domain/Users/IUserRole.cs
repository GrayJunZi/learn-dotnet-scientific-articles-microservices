using Articles.Abstractions.Enums;

namespace Auth.Domain.Users;

public interface IUserRole
{
    UserRoleType Type { get; }
    DateTime? StartDate { get; }
    DateTime? ExpiringDate { get; }
}