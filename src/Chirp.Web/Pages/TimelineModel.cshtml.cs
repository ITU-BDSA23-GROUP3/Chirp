using System.Security.Claims;
namespace Chirp.Web.Pages;

public class TimelineModel : PageModel
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    protected readonly ICheepService _service;
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ILikeRepository _likeRepository;
    protected readonly IFollowRepository _followRepository;
    public TimelineModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
    {
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
    }

    public int getUserId(string? authorName = null)
    {
        authorName ??= User.Identity.Name;
        
        var author = _authorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author == null ? 0 : author.AuthorId;
        /* 
        alternativt:
        return int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? "0"); 
        */
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string text = Request.Form["Text"];
        if (text.Length > 160) text = text.Substring(0, 160);
        
        var authorId = getUserId();
        if (authorId == 0) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?
        
        _service.StoreCheep( new Cheep {AuthorId = authorId, Text=text, TimeStamp = DateTime.Now} );
        return RedirectToPage();
    }

    public bool AuthorLikesCheep(string name, int cheepId)
    {
        var authorId = getUserId();
        return authorId != 0 && _likeRepository.LikeExists(authorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(string authorName, int cheepId)
    {
        var authorId = getUserId();
        return authorId != 0 && _likeRepository.LikesOwnCheep(authorId, cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int authorId = getUserId();
        _likeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int authorId = getUserId();
        _likeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public bool AuthorFollowsAuthor(string followerName, string followedName)
    {
        var followerId = getUserId(followerName);
        var followedId = getUserId(followedName);
        
        return followedId != 0 && followerId != 0 && _followRepository.FollowExists(followerId, followedId);
    }

    public int GetFollowersCount(string routeName)
    {
        var authorId = getUserId(routeName);
        if (authorId == 0) return 0;

        return _followRepository.FindFollowersCountByAuthorId(authorId);
    }

    public int GetFollowingCount(string routeName)
    {
        var authorId = getUserId(routeName);
        if (authorId == 0) return 0;

        return _followRepository.FindFollowingCountByAuthorId(authorId);
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        var followerId = getUserId();
        var followedId = getUserId(routeName);

        if (followerId == 0 || followedId == 0) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _followRepository.Follow(followerId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        var followerId = getUserId();
        var followedId = getUserId(routeName);

        if (followerId == 0 || followedId == 0) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _followRepository.Unfollow(followerId, followedId);
        return RedirectToPage();
    }

    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        NumOfCheeps = _service.GetCheepCount(author);

        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if ((page < 1 || page > maxPage) && _service.GetCheepCount(author) != 0)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page, author);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
