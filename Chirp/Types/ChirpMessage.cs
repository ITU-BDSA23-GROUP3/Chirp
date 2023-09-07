using System.Text.RegularExpressions;
using Chirp.Shared;

namespace Chirp.Types;

/// <summary>
/// A chirp message is a message that contains Author, Message and a Timestamp for when message is sent.
/// </summary>
public record ChirpMessage(string Author, string Message, long Timestamp)
{
    public override string ToString()
    {
        var now = DateTimeHelper.EpochToDateTime(Timestamp);
        return $"{Author} @ {now:MM/dd/yy hh:mm:ss}: {Message}";
    }
}