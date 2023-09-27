using Chirp.CLI.Shared;

namespace Chirp.CLI.Types;

/// <summary>
/// A chirp message is a message that contains Author, Message and a Timestamp for when message is sent.
/// </summary>
public record ChirpRecord(string author, string message, long timestamp)
{
    public override string ToString()
    {
        var now = DateTimeHelper.EpochToDateTime(timestamp);
        return $"{author} @ {now:MM/dd/yy hh:mm:ss}: {message}";
    }
}
