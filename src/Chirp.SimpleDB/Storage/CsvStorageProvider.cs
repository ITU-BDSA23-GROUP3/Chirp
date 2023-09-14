namespace Chirp.SimpleDB.Storage;

public abstract class CsvStorageProvider<T> : IStorageProvider<T>
{
    public static IStorage<T> Storage => new CsvStorage<T>("../../data/chirp_cli_db.csv");
}