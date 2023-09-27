namespace Chirp.CLI.Interfaces;

public interface IChirpHandler
{
    Task<int> HandleInput();
}
