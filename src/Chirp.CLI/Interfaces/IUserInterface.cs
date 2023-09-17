using Chirp.CLI.Types;

namespace Chirp.CLI.Interfaces;

public interface IUserInterface
{
    public void Read(IEnumerable<ChirpRecord> records);
    public void Help();
}