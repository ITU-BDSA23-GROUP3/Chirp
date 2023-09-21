using Chirp.CLI.Types;

namespace Chirp.CLI.Interfaces;

/// <summary>
/// User Interface interface
/// </summary>
public interface IUserInterface
{
    /// <summary>
    /// Read a chirp record 
    /// </summary>
    /// <param name="records"></param>
    /// <returns>Status</returns>
    public int Read(IEnumerable<ChirpRecord> records);

    /// <summary>
    /// Helps you:)
    /// </summary>
    /// <returns>Status</returns>
    public int Help();
}
