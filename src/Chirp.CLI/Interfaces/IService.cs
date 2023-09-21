namespace Chirp.CLI.Interfaces;

public interface IService<T, K>
{
    public Task<List<T>?> GetAllEntities();
    public Task<bool> StoreAllEntities(List<K> entities);
    public Task<bool> StoreEntity(K entity);
}