using Chirp.Types;

namespace Chirp.Storage;

public abstract class CsvStorageProvider<T> : IStorageProvider<T>
{
    public static IStorage<T> Storage => new CsvStorage<T>("../Data/chirp_cli_db.csv");
}