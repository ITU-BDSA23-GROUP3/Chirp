using Chirp.Types;

namespace Chirp.Parsers;

public class ChirpCsvParser
{
    private readonly IFileReader _fileReader;

    public ChirpCsvParser(IFileReader fileReader)
    {
        _fileReader = fileReader;
    }

    public async Task<List<ChirpMessage>> ParseAsync(CancellationToken cancellationToken)
    {
        return GetChirpsFromCsvString(await _fileReader.ParseAsync(cancellationToken));
    }

    private static List<ChirpMessage> GetChirpsFromCsvString(string csvString)
    {
        var chirpMessages = new List<ChirpMessage>();
        var rows = csvString.Split('\n');
        
        foreach (var row in rows[1..])
        {
            if (row.Trim() != "")
                accumulator.Add(new ChirpMessage(row));
        }

        return chirpMessages;
    }
}