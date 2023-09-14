namespace Chirp.CLI.Shared;

public class DateTimeHelper
{
    public static long DateTimeToEpoch(DateTime dateTime)
    {
        var start = dateTime - new DateTime(1970, 1, 1);
        return (long)start.TotalSeconds;
    }
    
    public static DateTime EpochToDateTime(long epoch)
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return start.AddSeconds(epoch);
    }
}
