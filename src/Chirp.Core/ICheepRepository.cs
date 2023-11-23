namespace Chirp.Core;

public interface ICheepRepository
{
    public void StoreCheep(CreateCheepDTO cheep);
    public void StoreCheeps(List<Cheep> entities);
    public IEnumerable<CheepDTO> QueryCheeps(int pageNumber, int amount, string? author = null, bool isAuthor = false);
    public int QueryCheepCount(string? author = null, bool isAuthor = false);
}
