using Chirp.CLI.Shared;

namespace Chirp.CLI.Types;

/// <summary>
/// A chirp message is a message that contains Author, Message and a Timestamp for when message is sent.
/// </summary>
public record ChirpRecord(string Author, string Message, long Timestamp)
{
    /// <summary>
    /// Displays a Chirp Record as a string.
    /// 
    /// Example: Bob @ 09/21/23 14:43:37: Hello World!
    /// </summary>
    /// <returns>String of the form "author @ MM/dd/yy hh:mm:ss: message"</returns>
    public override string ToString()
    {
        var now = DateTimeHelper.EpochToDateTime(Timestamp);
        return $"{Author} @ {now:MM/dd/yy hh:mm:ss}: {Message}";
    }
}
