using System.ComponentModel;

namespace Articles.Abstractions.Enums;

public enum UserRoleType
{
    // Cross Domain: 1-9
    [Description("Editorial Office")] EOF = 1,

    // Submission: 11-19
    [Description("Author")] AUT = 11,
    [Description("Corresponding Author")] CORAUT = 12,
}

public static class Role
{
    public const string EOF = nameof(UserRoleType.EOF);
    public const string AUT = nameof(UserRoleType.AUT);
    public const string CORAUT = nameof(UserRoleType.CORAUT);
}