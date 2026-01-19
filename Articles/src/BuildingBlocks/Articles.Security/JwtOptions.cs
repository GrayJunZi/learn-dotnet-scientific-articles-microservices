namespace Articles.Security;

public class JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
    public int ValidForInMinutes { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan ValidFor => TimeSpan.FromMinutes(ValidForInMinutes);
    
    public DateTime Expiration => IssuedAt.Add(ValidFor);
}