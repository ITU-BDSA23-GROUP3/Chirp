namespace Chirp.Razor.Shared.Storage;
public interface IPathHandler
{
    public string DefaultDataPath { get; }
    public string ChirpDbPath { get; }

    public string Combine(string first, string second);
}
public class PathHandler : IPathHandler
{
    public string DefaultDataPath { get; } = "../../data";
    public string ChirpDbPath { get; } = Path.Combine(Path.GetTempPath(), "chirp.db");

    public PathHandler(string? chirpDbPath = null, string? defaultDataPath = null) 
    {
        if (!string.IsNullOrEmpty(defaultDataPath))
        {
            DefaultDataPath = defaultDataPath;
        }

        // Allows for easy testing, using virtual filesystems
        if (!string.IsNullOrEmpty(chirpDbPath)) 
        {
            ChirpDbPath = chirpDbPath;
        }
        else 
        {
            string chirpDbEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            if (!string.IsNullOrEmpty(chirpDbEnv)) 
            {
                ChirpDbPath = chirpDbEnv;
            }
        }
    }

    public string Combine(string first, string second)
    {
        return Path.Combine(first, second); 
    }
}