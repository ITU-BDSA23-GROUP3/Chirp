namespace Chirp.Infrastructure.Storage;

public interface IStoragePathHandler
{
    public string DefaultDataPath { get; }
    public string ChirpDbPath { get; }

    public string Combine(string first, string second);
}
public class StoragePathHandler : IStoragePathHandler
{
    public string DefaultDataPath { get; } = "../../data";
    public string ChirpDbPath { get; } = getPathToLocalFolder();

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
        else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CHIRPDBPATH")))
        {
            ChirpDbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        }
        else if (Path.Exists(DefaultDataPath))
        {
            ChirpDbPath = Path.Combine(DefaultDataPath, "chirp.db");
        }
    }

    public static string getPathToLocalFolder()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        return System.IO.Path.Join(path, "chirp.db");
    }

    public string Combine(string first, string second)
    {
        return Path.Combine(first, second);
    }
}
