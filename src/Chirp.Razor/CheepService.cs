using Chirp.Razor.Storage;
using Chirp.Razor.Storage.Types;

namespace Chirp.Razor;

public interface ICheepService
{
    public int CheepsPerPage { get; }
    public List<Cheep> GetCheeps(int pageNumber);
    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author);
}

public class CheepService : ICheepService
{
    private readonly IChirpStorage _chirpStorage;

    public int CheepsPerPage => 5;

    public CheepService(IChirpStorage chirpStorage)
    {
        _chirpStorage = chirpStorage;
    }

    public List<Cheep> GetCheeps(int pageNumber)
    {
        return _chirpStorage.GetCheepsPerPage(pageNumber, CheepsPerPage).ToList();
    }

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author)
    {
        return _chirpStorage.GetCheepsFromAuthor(pageNumber, author);
    }
}
