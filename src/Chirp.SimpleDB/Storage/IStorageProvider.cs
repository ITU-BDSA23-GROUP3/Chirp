namespace Chirp.SimpleDB.Storage;

public interface IStorageProvider<T>
{
    public IStorage<T> Storage { get; }
}
