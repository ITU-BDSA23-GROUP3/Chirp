using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;

namespace Chirp.CLI.Storage;

public abstract class ChirpStorageProvider : IStorageProvider<ChirpMessage>
{
    private static IStorage<ChirpMessage>? _storage;
    
    public static IStorage<ChirpMessage> Storage
    {
        // This makes sure that our storage is a singleton so we dont open multiple instances of the file
        // We need to find a better way to provide the file perhaps a configuration file
        get { return _storage ??= new CsvStorage<ChirpMessage>("../../data/chirp_cli_db.csv"); }
    }
}