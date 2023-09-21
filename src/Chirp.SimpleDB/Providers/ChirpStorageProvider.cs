using Chirp.SimpleDB.Storage;
using Chirp.SimpleDB.Types;

namespace Chirp.SimpleDB.Providers;

public class ChirpStorageProvider : IStorageProvider<StorageChirpRecord>
{
    private static IStorage<StorageChirpRecord>? _storage;
    
    public IStorage<StorageChirpRecord> Storage
    {
        // This makes sure that our storage is a singleton so we dont open multiple instances of the file
        // We need to find a better way to provide the file perhaps a configuration file
        get { return _storage ??= new CsvStorage<StorageChirpRecord>("../../data/chirp_cli_db.csv"); }
    }
}
