namespace Chirp.Web.Pages;

public class TimelineModel : PageModel
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    public int CurrentPage = 1;
    public int MaxCharacterCount = 160;
    public string RouteName = "";
    protected readonly ICheepService _service;
    protected readonly IRepositoryManager _repositoryManager;
    public TimelineModel(ICheepService service, IRepositoryManager repositoryManager)
    {
        _service = service;
        _repositoryManager = repositoryManager;
    }

    public int? GetUserId(string? authorName = null)
    {
        authorName ??= User?.Identity?.Name;
        if (authorName == null) return null;
        
        var author = _repositoryManager.AuthorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author?.AuthorId;
    }

    public bool IsUserAuthenticated(){
        return User?.Identity != null && User.Identity.IsAuthenticated;
    }

    public IActionResult OnPost()
    {
        var authorId = GetUserId();
        if (authorId == null) return RedirectToPage();
        
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text.Substring(0, MaxCharacterCount);
        
        _service.StoreCheep( new Cheep {AuthorId = (int)authorId, Text=text, TimeStamp = DateTime.Now} );
        return RedirectToPage();
    }

    public bool AuthorLikesCheep(int cheepId)
    {
        var authorId = GetUserId();
        return authorId != null && _repositoryManager.LikeRepository.LikeExists((int)authorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _repositoryManager.LikeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(int cheepId)
    {
        var authorId = GetUserId();
        return authorId != null && _repositoryManager.LikeRepository.LikesOwnCheep((int)authorId, cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();

        var authorId = GetUserId();
        if (authorId == null) return Page();

        _repositoryManager.LikeRepository.LikeCheep((int)authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();

        var authorId = GetUserId();
        if (authorId == null) return Page();

        _repositoryManager.LikeRepository.UnlikeCheep((int)authorId, cheepId);
        return RedirectToPage();
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followerId = GetUserId();
        var followedId = GetUserId(followedName);
        
        return followedId != null && followerId != null && _repositoryManager.FollowRepository.FollowExists((int)followerId, (int)followedId);
    }

    public int GetFollowersCount(string routeName)
    {
        var authorId = GetUserId(routeName);
        if (authorId == null) return 0;

        return _repositoryManager.FollowRepository.FindFollowersCountByAuthorId((int)authorId);
    }

    public int GetFollowingCount(string routeName)
    {
        var authorId = GetUserId(routeName);
        if (authorId == null) return 0;

        return _repositoryManager.FollowRepository.FindFollowingCountByAuthorId((int)authorId);
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();

        var followerId = GetUserId();
        var followedId = GetUserId(routeName);

        if (followerId == null || followedId == null) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _repositoryManager.FollowRepository.Follow((int)followerId, (int)followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();

        var followerId = GetUserId();
        var followedId = GetUserId(routeName);

        if (followerId == null || followedId == null) return RedirectToPage(); // hvor/hvordan skal dette fejlhåndteres?

        _repositoryManager.FollowRepository.Unfollow((int)followerId, (int)followedId);
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
        var isAuthor = CalculateIsAuthor(author, User.Identity?.Name);
        NumOfCheeps = _service.GetCheepCount(author, isAuthor);

        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if ((page < 1 || page > maxPage) && NumOfCheeps != 0)
        {
            CurrentPage = 1;
            return RedirectToPage();
        }
        
        CurrentPage = page;
        
        Cheeps = _service.GetCheeps(page, author, isAuthor);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
