using System.Security.Claims;
namespace Chirp.Web.Pages;

public class TimelineModel : PageModel
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    public string RouteName = "";
    protected readonly ICheepService _service;
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ILikeRepository _likeRepository;
    protected readonly IFollowRepository _followRepository;
    protected readonly UserData _userData;
    public TimelineModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository, UserData userData)
    {
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
        _userData = userData;
    }

    public int GetUserId(string? authorName = null)
    {
        if (authorName == null) return 0;

        var author = _authorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author == null ? 0 : author.AuthorId;
    }

    public IActionResult OnPost()
    {
        if (_userData.Author == null) return RedirectToPage();

        string text = Request.Form["Text"].ToString();
        if (text.Length > 160) text = text.Substring(0, 160);

        _service.StoreCheep(new Cheep { AuthorId = _userData.Author.AuthorId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }

    public bool UserLikesCheep(int cheepId)
    {
        return _userData.Author != null && _likeRepository.LikeExists(_userData.Author.AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool UserLikesOwnCheep(int cheepId)
    {
        return _userData.Author != null && _likeRepository.LikesOwnCheep(_userData.Author.AuthorId, cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (_userData.Author == null) throw new Exception("Cannot like when logged out!");

        _likeRepository.LikeCheep(_userData.Author.AuthorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (_userData.Author == null) throw new Exception("Cannot unlike when logged out!");

        _likeRepository.UnlikeCheep(_userData.Author.AuthorId, cheepId);
        return RedirectToPage();
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followedId = GetUserId(followedName);

        return followedId != 0 && _userData.Author != null && _followRepository.FollowExists(_userData.Author.AuthorId, followedId);
    }

    public int GetFollowersCount()
    {
        var authorId = GetUserId(RouteName);
        if (authorId == 0) return 0;

        return _followRepository.FindFollowersCountByAuthorId(authorId);
    }

    public int GetFollowingCount()
    {
        var authorId = GetUserId(RouteName);
        if (authorId == 0) return 0;

        return _followRepository.FindFollowingCountByAuthorId(authorId);
    }

    public IActionResult OnPostFollow()
    {
        var followedId = GetUserId(HttpContext?.GetRouteValue("author")?.ToString());

        if (_userData.Author == null) throw new Exception("Cannot follow when logged out!");
        if (followedId == 0) throw new Exception("Trying to follow author that doesn't exist!");

        _followRepository.Follow(_userData.Author.AuthorId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow()
    {
        var followedId = GetUserId(HttpContext?.GetRouteValue("author")?.ToString());

        if (_userData.Author == null) throw new Exception("Cannot unfollow when logged out!");
        if (followedId == 0) throw new Exception("Trying to unfollow author that doesn't exist!");

        _followRepository.Unfollow(_userData.Author.AuthorId, followedId);
        return RedirectToPage();
    }
    
    public ActionResult OnGet([FromQuery] int page = 1)
    {
        RouteName = HttpContext?.GetRouteValue("author")?.ToString() ?? "";
        NumOfCheeps = _service.GetCheepCount(RouteName);

        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if ((page < 1 || page > maxPage) && _service.GetCheepCount(RouteName) != 0)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page, RouteName);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
