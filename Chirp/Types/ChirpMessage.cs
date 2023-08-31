using System.Text.RegularExpressions;

namespace Chirp.Types;

public partial record ChirpMessage
{
    public ChirpMessage(string csvString)
    {
        var row = MyRegex().Split(csvString);

        if (row.Length < 3)
            throw new ArgumentException(
                $"The string provided was {csvString} but was expecting 'string, string, long'");

        var msg = row[1];
        Author = row[0];
        Message = msg.Remove(0, 1).Remove(msg.Length - 2, 1);
        Timestamp = long.Parse(row[2]);
    }

    public ChirpMessage(string Author, string Message, long Timestamp)
    {
        this.Author = Author;
        this.Message = Message;
        this.Timestamp = Timestamp;
    }

    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }

    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
    private static partial Regex MyRegex();

    public override string ToString()
    {
        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var now = start.AddSeconds(Timestamp);
        return $"{Author} @ {now:MM/dd/yy hh:mm:ss}: {Message}";
    }

    public string ToCsvString()
    {
        var ts = Timestamp;
        return $"{Author},\"{Message}\",{ts}";
    }
}