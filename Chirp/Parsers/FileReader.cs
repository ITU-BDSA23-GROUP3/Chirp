using System.Text;

namespace Chirp.Parsers;

public class FileReader : IFileReader
{
    private readonly string _path;

    public FileReader(string path)
    {
        _path = path;
    }

    public async Task<string> ParseAsync(CancellationToken cancellationToken)
    {
        await using var fileStream = File.Open(_path, FileMode.Open);
        var buffer = new byte[fileStream.Length];
        await fileStream.ReadAsync(buffer.AsMemory(0, (int)fileStream.Length), cancellationToken);

        return Encoding.ASCII.GetString(buffer);
    }
}