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
    public TimelineModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
    {
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
    }

    public int? GetUserId(string? authorName = null)
    {
        authorName ??= User?.Identity?.Name;
        if (authorName == null) return null;
        
        var author = _authorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author?.AuthorId;
    }

    public bool IsUserAuthenticated(){
        return User?.Identity != null && User.Identity.IsAuthenticated;
    }

    public IActionResult OnPost()
    {
        var authorId = GetUserId();
        if (authorId == null) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?
        
        string text = Request.Form["Text"].ToString();
        if (text.Length > 160) text = text.Substring(0, 160);
        
        _service.StoreCheep( new Cheep {AuthorId = (int)authorId, Text=text, TimeStamp = DateTime.Now} );
        return RedirectToPage();
    }

    public bool AuthorLikesCheep(int cheepId)
    {
        var authorId = GetUserId();
        return authorId != null && _likeRepository.LikeExists((int)authorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(int cheepId)
    {
        var authorId = GetUserId();
        return authorId != null && _likeRepository.LikesOwnCheep((int)authorId, cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();

        var authorId = GetUserId();
        if (authorId == null) return Page();

        _likeRepository.LikeCheep((int)authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();

        var authorId = GetUserId();
        if (authorId == null) return Page();

        _likeRepository.UnlikeCheep((int)authorId, cheepId);
        return RedirectToPage();
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followerId = GetUserId();
        var followedId = GetUserId(followedName);
        
        return followedId != null && followerId != null && _followRepository.FollowExists((int)followerId, (int)followedId);
    }

    public int GetFollowersCount(string routeName)
    {
        var authorId = GetUserId(routeName);
        if (authorId == null) return 0;

        return _followRepository.FindFollowersCountByAuthorId((int)authorId);
    }

    public int GetFollowingCount(string routeName)
    {
        var authorId = GetUserId(routeName);
        if (authorId == null) return 0;

        return _followRepository.FindFollowingCountByAuthorId((int)authorId);
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();

        var followerId = GetUserId();
        var followedId = GetUserId(routeName);

        if (followerId == null || followedId == null) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _followRepository.Follow((int)followerId, (int)followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();

        var followerId = GetUserId();
        var followedId = GetUserId(routeName);

        if (followerId == null || followedId == null) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _followRepository.Unfollow((int)followerId, (int)followedId);
        return RedirectToPage();
    }

    public bool IsUserOrPublicPage() {
        return RouteName == User.Identity?.Name || RouteName == "";
    }

    private bool CalculateIsAuthor(string? pageAuthor, string? loggedInUser) {
        if (pageAuthor == null|| loggedInUser == null) return false;
        return pageAuthor==loggedInUser;
    }

    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        RouteName = HttpContext.GetRouteValue("author")?.ToString() ?? "";
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
        
        Cheeps = _service.GetCheeps(page, author, CalculateIsAuthor(author, User.Identity?.Name));
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
