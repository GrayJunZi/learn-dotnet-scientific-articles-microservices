using Auth.Domain.Users.Enums;
using Auth.Domain.Users.ValueObjects;
using BuildingBlocks.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Users;

public partial class User : IdentityUser<int>, IEntity
{
    public required string Name { get; set; }

    public required Gender Gender { get; set; }
    public HonorificTitle? Honorific { get; set; }

    public string? Position { get; set; }
    public string? CompanyName { get; set; }
    public string? Affiliation { get; set; }

    public ProfessionalProfile? ProfessionalProfile { get; set; }

    public string? PictureUrl { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    private List<UserRole> _userRoles = [];
    public virtual IReadOnlyList<UserRole> UserRoles => _userRoles;

    private List<RefreshToken> _refreshTokens = [];
    public virtual  List<RefreshToken> RefreshTokens => _refreshTokens;
}