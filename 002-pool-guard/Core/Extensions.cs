namespace PoolGuard.Core;


public static class Extensions
{
    public static ZonedDateTime ToLocalDateTime(this Instant instant)
    {
        return instant.InZone(Const.TimeZone);
    }
}