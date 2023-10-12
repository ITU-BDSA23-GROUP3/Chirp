namespace Chirp.Razor.Storage;

public interface IChirpStorage
{
    public void StoreCheep(Cheep entity);
    
    public void StoreCheeps(List<Cheep> entities);
    
    public List<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null);

    public int QueryCheepCount(string? author = null);
}
