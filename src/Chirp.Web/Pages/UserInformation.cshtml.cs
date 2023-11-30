namespace Chirp.Web.Pages;
public class UserInformationModel : PageModel
{
    List<Cheep> Cheeps { get; set; } = new List<Cheep>();

    List<Author> Follows { get; set; } = new List<Author>();

    List <Like> Likes { get; set; } = new List<Like>();

    protected readonly ICheepService _service;
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ILikeRepository _likeRepository;
    protected readonly IFollowRepository _followRepository;

    public UserInformationModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
    {
        _service = service;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
    }
    
    public int? GetUserId()
    {
        var author = _authorRepository.FindAuthorsByName(User?.Identity?.Name).FirstOrDefault();
        return author?.AuthorId;
    }

    public string GetEmail(){
        var author = _authorRepository.FindAuthorsByName(User?.Identity?.Name).FirstOrDefault();
        return author?.Email;
    }

    public IEnumerable<Author>  GetFollowing(){
        var userId = GetUserId();
        if (userId == null) return null;
        var following = _followRepository.FindFollowingByAuthorId((int)userId);
        return _authorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    public IEnumerable<Author> GetFollowers(){
        var userId = GetUserId();
        if (userId == null) return null;
        var followers = _followRepository.FindFollowersByAuthorId((int)userId);
        return _authorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }

    public int GetCheepCount(){
        var userId = GetUserId();
        if (userId == null) return 0;
        return _service.GetCheepCount (User.Identity.Name);
    }

    public IEnumerable<Cheep> GetCheeps(){
        var userId = GetUserId();
        if (userId == null) return null;
        return _service.GetAllCheepsFromAuthor(User.Identity.Name);
    }

    public IEnumerable<Cheep> GetLikedCheeps()
    {
        var userId = GetUserId();
        if (userId == null) return null;
        var likes = _likeRepository.FindLikesByAuthorId((int)userId);
        return _service.GetCheeps(1).Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }

    public IActionResult OnPostDelete(string routeName)
    {
        var authorId = GetUserId();
        if (authorId == null) return RedirectToPage(); 
        _service.DeleteCheeps((List<Cheep>)GetCheeps());
        _followRepository.DeleteAllFollowsByAuthorId((int)authorId);
        _authorRepository.DeleteAuthor((int)authorId);
        _likeRepository.DeleteAllLikesByAuthorId((int)authorId);
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");

    }
}
