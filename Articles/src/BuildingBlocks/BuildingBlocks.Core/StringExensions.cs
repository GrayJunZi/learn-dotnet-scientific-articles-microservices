namespace BuildingBlocks.Core;

public static class StringExensions
{
    public static string FormatWith(this string @this, params object[] args)
        => string.Format(@this, args);

    public static string FormatWith(this string @this, object arg)
        => string.Format(@this, arg);
}