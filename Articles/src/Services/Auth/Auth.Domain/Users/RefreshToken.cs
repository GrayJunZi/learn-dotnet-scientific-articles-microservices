namespace Auth.Domain.Users;

public class RefreshToken
{
    public int UserId { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedByIP { get; set; }
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }
}