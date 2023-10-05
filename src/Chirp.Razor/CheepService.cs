using Chirp.Razor.Storage;
using Chirp.Razor.Storage.Types;

namespace Chirp.Razor;

public interface ICheepService
{
    public int CheepsPerPage { get; }
    public List<Cheep> GetCheeps(int pageNumber);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string author);
    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author);
}

public class CheepService : ICheepService
{
    private readonly IChirpStorage _chirpStorage;
    public int CheepsPerPage => 32;
    
    public CheepService(IChirpStorage chirpStorage)
    {
        _chirpStorage = chirpStorage;
    }

    public List<Cheep> GetCheeps(int pageNumber)
    {
        return _chirpStorage.GetCheepsPerPage(pageNumber, CheepsPerPage).ToList();
    }

    public int GetCheepCount()
    {
        return _chirpStorage.CountCheeps();
    }

    public int GetAuthorCheepCount(string author)
    {
        return _chirpStorage.CountCheepsFromAuthor(author);
    }

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author)
    {
        return _chirpStorage.GetCheepsFromAuthor(pageNumber, CheepsPerPage, author);
    }
}
