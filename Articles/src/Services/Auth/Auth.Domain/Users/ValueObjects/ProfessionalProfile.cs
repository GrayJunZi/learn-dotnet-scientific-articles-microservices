namespace Auth.Domain.Users.ValueObjects;

public class ProfessionalProfile :ValueObject
{
    public string? Position { get; set; }
    public string? CompanyName { get; set; }
    public string? Affiliation { get; set; }

    private ProfessionalProfile()
    {
    }

    public static ProfessionalProfile Create(string? position, string? companyName, string? affiliation)
    {
        return new ProfessionalProfile
        {
            Position = string.IsNullOrWhiteSpace(position) ? null : position.Trim(),
            CompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim(),
            Affiliation = string.IsNullOrWhiteSpace(affiliation) ? null : affiliation.Trim(),
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Position;
        yield return CompanyName;
        yield return Affiliation;
    }
}