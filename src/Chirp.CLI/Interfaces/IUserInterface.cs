using Chirp.CLI.Types;

namespace Chirp.CLI.Interfaces;

public interface IUserInterface
{
    public int Read(IEnumerable<ChirpRecord> records);
    public int Help();
}
