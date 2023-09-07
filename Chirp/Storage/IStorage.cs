namespace Chirp.Storage;

public interface IStorage<T>
{
    public List<T> Records { get; }
    public void StoreEntity(T entity);
    public void StoreEntities(List<T> entities);
    public T GetEntity();
    public IEnumerable<T> GetEntities();
}