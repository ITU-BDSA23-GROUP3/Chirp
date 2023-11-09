using Chirp.Core;

namespace Chirp.Web;

public interface ICheepService
{
    public int CheepsPerPage { get; }
    public List<Cheep> GetCheeps(int pageNumber, string? author = null);
    public int GetCheepCount(string? author = null);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _chirpStorage;
    public int CheepsPerPage => 32;
    
    public CheepService(ICheepRepository chirpStorage)
    {
        _chirpStorage = chirpStorage;
    }

    public List<Cheep> GetCheeps(int pageNumber, string? author = null)
    {
        return _chirpStorage.QueryCheeps(pageNumber, CheepsPerPage, author).ToList();
    }

    public int GetCheepCount(string? author = null)
    {
        return _chirpStorage.QueryCheepCount(author);
    }
}
