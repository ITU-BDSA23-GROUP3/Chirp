namespace Chirp.CLI.Interfaces;

public interface IServiceProvider<T, K>
{
    public IService<T, K> Service { get; }
}
