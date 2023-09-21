namespace Chirp.CLI.Interfaces;

/// <summary>
/// Interface for the Chirp Handler
/// </summary>
public interface IChirpHandler
{
    /// <summary>
    /// Handle input from CLI arguments
    /// </summary>
    /// <returns>Status</returns>
    int HandleInput();
}
