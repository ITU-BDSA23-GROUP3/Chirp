using System.Text;

namespace Chirp.Writers;

public class FileWriter : IFileWriter
{
    private readonly string _path;
    private readonly StringBuilder _linesToWrite;

    public FileWriter(string path)
    {
        _path = path;
        _linesToWrite = new StringBuilder();
    }

    public void AddLine(string line)
    {
        _linesToWrite.AppendLine(line);
    }

    public async Task WriteAsync(CancellationToken cancellationToken)
    {
        await using var fileStream = File.Open(_path, FileMode.Open);
        fileStream.Seek(0, SeekOrigin.End);
        var stringToAppend = Encoding.ASCII.GetBytes(_linesToWrite.ToString());
        await fileStream.WriteAsync(stringToAppend, cancellationToken);
    }
}