namespace Chirp.Parsers;

public interface IFileReader
{
    public Task<string> ParseAsync(CancellationToken cancellationToken);
}