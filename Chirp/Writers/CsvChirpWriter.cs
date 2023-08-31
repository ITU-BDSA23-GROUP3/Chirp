using Chirp.Types;

namespace Chirp.Writers;

public class CsvChirpWriter
{
    private readonly IFileWriter _fileWriter;

    public CsvChirpWriter(IFileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public void WriteLine(ChirpMessage chirp)
    {
        _fileWriter.AddLine(chirp.ToCsvString());
    }

    public async Task FlushToFile(CancellationToken cancellationToken)
    {
        await _fileWriter.WriteAsync(cancellationToken);
    }
}