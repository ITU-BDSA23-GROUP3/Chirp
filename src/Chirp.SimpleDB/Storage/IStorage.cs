namespace Chirp.SimpleDB.Storage;

/// <summary>
/// Interface for IStorage using generic types, in case what we want to store changes
/// </summary>
/// <typeparam name="T">Arbitrary parameter</typeparam>
public interface IStorage<T>
{
    public List<T> Records { get; }
    public void StoreEntity(T entity);
    public void StoreEntities(List<T> entities);
    public T GetEntity();
    public IEnumerable<T> GetEntities();
}