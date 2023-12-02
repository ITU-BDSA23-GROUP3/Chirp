namespace Chirp.Core;

public interface ICheepRepository
{
    public void StoreCheep(Cheep cheep);
    public void StoreCheeps(List<Cheep> entities);
    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null, bool isAuthor = false);
    public int QueryCheepCount(string? author = null, bool isAuthor = false);
    public void DeleteCheep(Cheep cheep);
    public void DeleteAllCheepsByAuthorId(int authorId) ;
}
