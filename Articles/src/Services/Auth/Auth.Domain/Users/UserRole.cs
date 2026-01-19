using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Users;

public partial class UserRole : IdentityUserRole<int>
{
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiringDate { get; set; }
}