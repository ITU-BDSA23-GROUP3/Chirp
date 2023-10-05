namespace Chirp.Razor.Shared.Storage;
public interface IStoragePathHandler
{
    public string DefaultDataPath { get; }
    public string ChirpDbPath { get; }

    public string Combine(string first, string second);
}
public class StoragePathHandler : IStoragePathHandler
{
    public string DefaultDataPath { get; } = Path.Combine("..", "..", "data");
    public string ChirpDbPath { get; } = Path.Combine(Path.GetTempPath(), "chirp.db");

    public StoragePathHandler(string? chirpDbPath = null, string? defaultDataPath = null) 
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
        else if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CHIRPDBPATH"))) 
        {
            ChirpDbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        }
        else if (Path.Exists(DefaultDataPath))
        {
            ChirpDbPath = Path.Combine(DefaultDataPath, "chirp.db");
        }
    }

    public string Combine(string first, string second)
    {
        return Path.Combine(first, second); 
    }
}
