namespace Chirp.Storage;

public interface IStorageProvider<T>
{
    #pragma warning disable
    public static IStorage<T> Storage { get; }
    #pragma warning restore
}