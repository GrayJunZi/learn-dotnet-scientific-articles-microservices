using Articles.Abstractions.Enums;
using BuildingBlocks.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Role;

public class Role : IdentityRole<int>, IEntity
{
    public required UserRoleType Type { get; set; }
    public required string Description { get; set; }
}