using Chirp.Core;

namespace Chirp.Web;

public interface ICheepService
{
    public int CheepsPerPage { get; }
    public List<CheepDTO> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false);
    public int GetCheepCount(string? author = null);
    public void StoreCheep(CreateCheepDTO cheep);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _chirpStorage;
    public int CheepsPerPage => 32;

    public CheepService(ICheepRepository chirpStorage)
    {
        _chirpStorage = chirpStorage;
    }

    public List<CheepDTO> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false)
    {
        return _chirpStorage.QueryCheeps(pageNumber, CheepsPerPage, author, isAuthor).ToList();
    }

    public int GetCheepCount(string? author = null)
    {
        return _chirpStorage.QueryCheepCount(author);
    }

    public void StoreCheep(CreateCheepDTO cheep)
    {
        _chirpStorage.StoreCheep(cheep with { TimeStamp = DateTime.Now });
    }
}
