using System.IO.Pipelines;
using Auth.Domain.Users.ValueObjects;

namespace Auth.Domain.Users;

public partial class User
{
    public static User Create(IUserCreationInfo userCreationInfo)
    {
        if (userCreationInfo.UserRoles is not { Count: > 0 })
            throw new ArgumentException("User must have at least one role.", nameof(userCreationInfo.UserRoles));

        var user = new User
        {
            UserName = userCreationInfo.Email,
            Email = userCreationInfo.Email,
            Name = userCreationInfo.Name,
            Gender = userCreationInfo.Gender,
            PhoneNumber = userCreationInfo.PhoneNumber,
            PictureUrl = userCreationInfo.PictureUrl,
            Honorific = HonorificTitle.FromEnum(userCreationInfo.Honorific),
            ProfessionalProfile = ProfessionalProfile.Create(
                userCreationInfo.Position,
                userCreationInfo.CompanyName,
                userCreationInfo.Affiliation
            ),
            _userRoles = userCreationInfo.UserRoles
                .Select(x => UserRole.Create(x))
                .ToList(),
        };

        return user;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }
}