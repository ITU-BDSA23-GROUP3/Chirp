using Chirp.Types;

namespace Chirp.Writers;

public interface IFileWriter
{
    public void AddLine(ICsvSerializable serializable);
    public Task WriteAsync(CancellationToken cancellationToken);
}