namespace Chirp.Writers;

public interface IFileWriter
{
    public void AddLine(string line);
    public Task WriteAsync(CancellationToken cancellationToken);
}