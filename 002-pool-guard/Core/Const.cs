namespace PoolGuard.Core;

public static class Const
{
    public const int MaxCapacity = 9;
    public static readonly DateTimeZone TimeZone = DateTimeZoneProviders.Tzdb["Europe/Vienna"];
    public static readonly LocalTime OpeningTime = new(08, 00, 00);
    public static readonly LocalTime ClosingTime = new(17, 15, 00);

}