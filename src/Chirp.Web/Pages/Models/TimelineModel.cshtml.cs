namespace Chirp.Web.Pages.Models;

public class TimelineModel : ChirpModel
{
    public string RouteName = "";
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int NumOfCheeps;
    public int CheepsPerPage = 32;
    public int CurrentPage = 1;
    public int MaxCharacterCount = 160;
    
    public TimelineModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) { }

    public bool IsUserOrPublicPage(string routeName)
    {
        return routeName == GetUserName() || routeName == "";
    }

    protected bool CalculateIsAuthor(string? pageAuthor, string? loggedInUser) {
        if (pageAuthor == null|| loggedInUser == null) return false;
        return pageAuthor==loggedInUser;
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(followedName).AuthorId;
        return _repositoryManager.FollowRepository.FollowExists(followerId, followedId);
    }

    public bool UserLikesCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikeExists(GetAuthor().AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _repositoryManager.LikeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikesOwnCheep(GetAuthor().AuthorId, cheepId);
    }

    public int GetFollowersCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowersCountByAuthorId(GetAuthor(routeName).AuthorId);
    }

    public int GetFollowingCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowingCountByAuthorId(GetAuthor(routeName).AuthorId);
    }

    public IActionResult OnPost()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text[..MaxCharacterCount];
        _repositoryManager.CheepRepository.StoreCheep(new Cheep { AuthorId = authorId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        _repositoryManager.LikeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        _repositoryManager.LikeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(routeName).AuthorId;
        _repositoryManager.FollowRepository.Follow(followerId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(routeName).AuthorId;
        _repositoryManager.FollowRepository.Unfollow(followerId, followedId);
        return RedirectToPage();
    }

    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        RouteName = HttpContext.GetRouteValue("author")?.ToString() ?? "";
        var isAuthor = CalculateIsAuthor(author, GetUserName());
        NumOfCheeps = _repositoryManager.CheepRepository.GetQueryableCheeps(author, isAuthor).Count();

        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / CheepsPerPage);

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
        Cheeps = _repositoryManager.CheepRepository.GetCheepsPaginated(page, CheepsPerPage, author, isAuthor).ToList();
        return Page();
    }
}
