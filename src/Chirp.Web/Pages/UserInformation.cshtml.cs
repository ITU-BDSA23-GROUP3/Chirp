
namespace Chirp.Web.Pages;

public class UserInformationModel : PageModel
{
    List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    List<Author> Follows { get; set; } = new List<Author>();
    List<Like> Likes { get; set; } = new List<Like>();

    protected readonly ICheepService _service;
    protected readonly IRepositoryManager _repositoryManager;

    public UserInformationModel(IRepositoryManager repositoryManager, ICheepService service)
    {
        _service = service;
        _repositoryManager = repositoryManager;
    }
    
    public int? GetUserId()
    {
        var author = _repositoryManager.AuthorRepository.FindAuthorsByName(User?.Identity?.Name).FirstOrDefault();
        return author?.AuthorId;
    }

    public string GetEmail()
    {
        var author = _repositoryManager.AuthorRepository.FindAuthorsByName(User?.Identity?.Name).FirstOrDefault();
        return author?.Email;
    }

    public IEnumerable<Author> GetFollowing()
    {
        var userId = GetUserId();
        if (userId == null) return new List<Author>();
        var following = _repositoryManager.FollowRepository.FindFollowingByAuthorId((int)userId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    public IEnumerable<Author> GetFollowers()
    {
        var userId = GetUserId();
        if (userId == null) return new List<Author>();
        var followers = _repositoryManager.FollowRepository.FindFollowersByAuthorId((int)userId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }

    public int GetCheepCount()
    {
        var userId = GetUserId();
        if (userId == null) return 0;
        return _service.GetCheepCount(User.Identity.Name);
    }

    public IEnumerable<Cheep> GetCheeps()
    {
        var userId = GetUserId();
        if (userId == null) return new List<Cheep>();
        return _service.GetAllCheepsFromAuthor(User.Identity.Name);
    }

    public IEnumerable<Cheep> GetLikedCheeps()
    {
        var userId = GetUserId();
        if (userId == null) return new List<Cheep>();
        var likes = _repositoryManager.LikeRepository.FindLikesByAuthorId((int)userId);
        return _service.GetCheeps(1).Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }

    public IActionResult OnPostDelete(string routeName)
    {
        var authorId = GetUserId();
        if (authorId == null) return RedirectToPage(); 

        //delete all cheeps, likes and follows
        _repositoryManager.FollowRepository.DeleteAllFollowsByAuthorId((int)authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesByAuthorId((int)authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesOnCheepsByAuthorId((int)authorId);
        _repositoryManager.AuthorRepository.DeleteAuthor((int)authorId);
        _service.DeleteCheeps((int)authorId);

        //logout
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");

    }
}
