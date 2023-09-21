using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;

namespace Chirp.CLI.Storage;

public class ChirpStorageProvider : IStorageProvider<ChirpRecord>
{
    private static IStorage<ChirpRecord>? _storage;
    
    /// <summary>
    /// Return _storage, if _storage isn't instantiated, create an object, set _storage, and return that
    /// </summary>
    public IStorage<ChirpRecord> Storage
    {
        // This makes sure that our storage is a singleton so we dont open multiple instances of the file
        // We need to find a better way to provide the file perhaps a configuration file
        get { return _storage ??= new CsvStorage<ChirpRecord>("../../data/chirp_cli_db.csv"); }
    }
}
