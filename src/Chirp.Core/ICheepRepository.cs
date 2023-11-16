namespace Chirp.Core;

public interface ICheepRepository
{
    public void StoreCheep(int authorId, string text);
    public void StoreCheeps(List<Cheep> entities);
    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null);
    public int QueryCheepCount(string? author = null);
}
