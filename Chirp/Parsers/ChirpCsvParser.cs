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
        var accumulator = new List<ChirpMessage>();
        var rows = csvString.Split('\n');

        var first = true;
        foreach (var row in rows)
        {
            if (!first && row.Trim() != "")
                accumulator.Add(new ChirpMessage(row));
            first = false;
        }

        return accumulator;
    }
}