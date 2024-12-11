public static class TimeZoneHelper
{
    private static readonly TimeZoneInfo ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

    public static DateTime ConvertClientTimeToServerTime(DateTime clientDateTime, string clientTimeZoneId)
    {
        var clientDateTimeUnspecified = DateTime.SpecifyKind(clientDateTime, DateTimeKind.Unspecified);

        var clientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId);

        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(clientDateTimeUnspecified, clientTimeZone);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ServerTimeZone);
    }

    public static DateTime ConvertServerTimeToClientTime(DateTime serverDateTime, string clientTimeZoneId)
    {
        var utcDateTime = DateTime.SpecifyKind(serverDateTime, DateTimeKind.Utc);

        var clientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, clientTimeZone);
    }
}