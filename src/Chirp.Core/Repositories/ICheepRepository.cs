namespace Chirp.Core;

public interface ICheepRepository
{
    void StoreCheep(Cheep cheep);
    void StoreCheeps(List<Cheep> entities);
    IEnumerable<Cheep> GetCheepsPaginated(int pageNumber, int cheepsPerPage, string? author = null, bool isUser = false);
    IQueryable<Cheep> GetQueryableCheeps(string? author = null, bool isUser = false);
    IQueryable<Cheep> GetAllCheepsByAuthorAndFollowers(int authorId);
    IQueryable<Cheep> GetAll();
    IQueryable<Cheep> GetAllCheepsByAuthorId(int authorId);
    void DeleteAllCheepsByAuthorId(int authorId);
    void DeleteCheep(Cheep cheep);
    
}
