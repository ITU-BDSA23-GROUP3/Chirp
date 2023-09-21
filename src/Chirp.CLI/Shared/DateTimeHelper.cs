namespace Chirp.CLI.Shared;

/// <summary>
/// Helper class for Epoch, and DateTime conversion
/// </summary>
public class DateTimeHelper
{
    /// <summary>
    /// Converts the DateTime to a UNIX epoch timestamp and returns that
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>long epoch version of datetime</returns>
    public static long DateTimeToEpoch(DateTime dateTime)
    {
        var start = dateTime - new DateTime(1970, 1, 1);
        return (long)start.TotalSeconds;
    }
    
    /// <summary>
    /// Converts the UNIX epoch timestamp as DateTime
    /// </summary>
    /// <param name="epoch"></param>
    /// <returns>Epoch as DateTime</returns>
    public static DateTime EpochToDateTime(long epoch)
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return start.AddSeconds(epoch);
    }
}
