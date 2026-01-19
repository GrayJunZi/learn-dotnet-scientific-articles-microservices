namespace BuildingBlocks.Core;

public static class DateTimeExtensions
{
    public static long ToUnixEpocDate(this DateTime dateTime)
        => (long)Math.Round((dateTime.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);
}