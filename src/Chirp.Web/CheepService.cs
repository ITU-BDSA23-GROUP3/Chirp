using Chirp.Core;

namespace Chirp.Web;

public interface IChirpService
{
    public int CheepsPerPage { get; }
    public List<Cheep> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false);
    public int GetCheepCount(string? author = null, bool isAuthor = false);
    public void StoreCheep(Cheep cheep);
    public IEnumerable<Cheep> GetAllCheepsFromAuthor(string author);
    public void DeleteCheep(Cheep cheep);
    public void DeleteCheeps(int authorId);
}

public class ChirpService : IChirpService
{
    private readonly IRepositoryManager _repositoryManager;
    public int CheepsPerPage => 32;

    public ChirpService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public List<Cheep> GetCheeps(int pageNumber, string? author = null, bool isAuthor = false)
    {
        return _repositoryManager.CheepRepository.QueryCheeps(pageNumber, CheepsPerPage, author, isAuthor).ToList();
    }

    public int GetCheepCount(string? author = null, bool isAuthor = false)
    {
        return _repositoryManager.CheepRepository.QueryCheepCount(author, isAuthor);
    }

    public void StoreCheep(Cheep cheep)
    {
        _repositoryManager.CheepRepository.StoreCheep(new Cheep { AuthorId = cheep.AuthorId, Text = cheep.Text, TimeStamp = DateTime.Now });
    }

    public IEnumerable<Cheep> GetAllCheepsFromAuthor(string author)
    {

        // This can be cleaned up by moving the pagenumber parameter to the web project
        return _repositoryManager.CheepRepository.QueryCheeps(1, 10000, author, false).ToList();
    }

    public void DeleteCheep(Cheep cheep)
    {
        _repositoryManager.CheepRepository.DeleteCheep(cheep);
    }

    public void DeleteCheeps(int authorId)
    {
        _repositoryManager.CheepRepository.DeleteAllCheepsByAuthorId(authorId);
    }
}
