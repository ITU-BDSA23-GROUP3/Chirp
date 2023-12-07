namespace Chirp.Web.Pages.Models;

public class ChirpModel : PageModel
{
    protected readonly IRepositoryManager _repositoryManager;

    // Can be moved to timelinemodel once cheeprepository is fixed
    public int CheepsPerPage => 32;

    public ChirpModel(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public string? GetUserName() {
        return User?.Identity?.Name;
    }

    public Author GetAuthor(string? authorName = null)
    {
        authorName ??= GetUserName();
        if (authorName == null) throw new Exception("User is not authenticated!");
        
        var author = _repositoryManager.AuthorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author ?? throw new Exception("Author doesn't exist!");
    }

    public bool IsUserAuthenticated(){
        return User.Identity?.IsAuthenticated == true;
    }

    public IEnumerable<Author> GetFollowing(){
        var following = _repositoryManager.FollowRepository.FindFollowingByAuthorId(GetAuthor().AuthorId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    public IEnumerable<Author> GetFollowers(){
        var followers = _repositoryManager.FollowRepository.FindFollowersByAuthorId(GetAuthor().AuthorId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }
}
