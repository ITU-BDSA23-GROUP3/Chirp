using System.Text.RegularExpressions;
using Chirp.Shared;

namespace Chirp.Types;

/// <summary>
/// A chirp message is a message that contains Author, Message and a Timestamp for when message is sent.
/// </summary>
public partial record ChirpMessage : ICsvSerializable
{
    
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
    
    /// <inheritdoc cref="ChirpMessage"/>
    /// <param name="csvString"></param>
    /// <exception cref="ArgumentException"></exception>
    public ChirpMessage(string csvString)
    {
        var contents = IgnoreCommasBetweenQuotesRegex().Split(csvString);

        if (contents.Length < 3)
            throw new ArgumentException(
                $"The string provided was {csvString} but was expecting 'string, string, long'");

        var message = contents[(int)ChirpFormatCsv.Message];
        Author = contents[(int)ChirpFormatCsv.Author];
        // Removes the quotation marks from a message as it is received as "message"
        Message = message[1..(message.Length - 1)];
        Timestamp = long.Parse(contents[(int)ChirpFormatCsv.TimeStamp]);
    }

    public ChirpMessage(string Author, string Message, long Timestamp)
    {
        this.Author = Author;
        this.Message = Message;
        this.Timestamp = Timestamp;
    }
    
    // From: https://stackoverflow.com/questions/46284336/ignore-comma-between-double-quotes-while-reading-csv-file
    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
    private static partial Regex IgnoreCommasBetweenQuotesRegex();

    public override string ToString()
    {
        var now = DateTimeHelper.EpochToDateTime(Timestamp);
        return $"{Author} @ {now:MM/dd/yy hh:mm:ss}: {Message}";
    }

    public string ToCsvString()
    {
        return $"{Author},\"{Message}\",{Timestamp}";
    }
}