using System.Text.RegularExpressions;
using BuildingBlocks.Core;
using BuildingBlocks.Domain.ValueObjects;

namespace Submission.Domain.ValueObjects;

public class EmailAddress : StringValueObject
{
    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string value)
    {
        Guard.ThrowIfNullOrWhiteSpace(value);
        if (!IsValidEmail(value))
            throw new ArgumentException($"Invalid email address '{value}'");

        return new EmailAddress(value);
    }

    private static bool IsValidEmail(string email)
    {
        const string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }
}