namespace Chirp.Web.Storage;

public interface IChirpRepository
{
    public void StoreCheep(Cheep entity);
    
    public void StoreCheeps(List<Cheep> entities);
    
    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null);

    public int QueryCheepCount(string? author = null);
}