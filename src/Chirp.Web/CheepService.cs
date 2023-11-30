using Chirp.Core;

namespace Chirp.Web;

public interface ICheepService
{
    public int CheepsPerPage { get; }
    public List<Cheep> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false);
    public int GetCheepCount(string? author = null, bool isAuthor = false);
    public void StoreCheep(Cheep cheep);
    public IEnumerable<Cheep> GetAllCheepsFromAuthor(string author);
    public void DeleteCheep(Cheep cheep);
    public void DeleteCheeps(List<Cheep> cheeps);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _chirpStorage;
    public int CheepsPerPage => 32;

    public CheepService(ICheepRepository chirpStorage)
    {
        _chirpStorage = chirpStorage;
    }

    public List<Cheep> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false)
    {
        return _chirpStorage.QueryCheeps(pageNumber, CheepsPerPage, author, isAuthor).ToList();
    }

    public int GetCheepCount(string? author = null, bool isAuthor = false)
    {
        return _chirpStorage.QueryCheepCount(author, isAuthor);
    }

    public void StoreCheep(Cheep cheep)
    {
        _chirpStorage.StoreCheep(new Cheep { AuthorId = cheep.AuthorId, Text = cheep.Text, TimeStamp = DateTime.Now });
    }

    public IEnumerable<Cheep> GetAllCheepsFromAuthor(string author)
    {
        return _chirpStorage.QueryCheeps(1, 10000, author, false).ToList();
    }

    public void DeleteCheep(Cheep cheep)
    {
        _chirpStorage.DeleteCheep(cheep);
    }

    public void DeleteCheeps(List<Cheep> cheeps)
    {
        _chirpStorage.DeleteCheeps(cheeps);
    }
}
