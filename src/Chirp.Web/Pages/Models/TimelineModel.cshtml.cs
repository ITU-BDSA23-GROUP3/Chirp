namespace Chirp.Web.Pages.Models;

public class TimelineModel : ChirpModel
{
    public string RouteName = "";
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int NumOfCheeps;
    public int CurrentPage = 1;
    public int MaxCharacterCount = 160;
    public TimelineModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) { }

    public bool IsUserOrPublicPage(string routeName)
    {
        return routeName == GetUserName() || routeName == "";
    }

    protected bool CalculateIsAuthor(string? pageAuthor, string? loggedInUser)
    {
        if (pageAuthor == null || loggedInUser == null) return false;
        return pageAuthor == loggedInUser;
    }

    public bool UserFollowsAuthor(string routeName)
    {
        return _repositoryManager.FollowRepository.FollowExists(
            new Follow { FollowerId = GetAuthor().AuthorId, FollowedId = GetAuthor(routeName).AuthorId }
        );
    }

    public bool UserLikesCheep(Cheep cheep)
    {
        return _repositoryManager.LikeRepository.LikeExists(new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId });
    }

    public int GetLikeCount(Cheep cheep)
    {
        return _repositoryManager.LikeRepository.FindLikeCountByCheep(cheep);
    }

    public bool LikesOwnCheep(Cheep cheep)
    {
        return _repositoryManager.LikeRepository.LikesOwnCheep(new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId });
    }

    public int GetFollowersCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowersCountByAuthor(GetAuthor(routeName));
    }

    public int GetFollowingCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowingCountByAuthor(GetAuthor(routeName));
    }

    public IActionResult OnPost()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text[..MaxCharacterCount];
        _repositoryManager.CheepRepository.StoreCheep(new Cheep { AuthorId = authorId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }

    public IActionResult OnPostLike(Cheep cheep)
    {
        if (!IsUserAuthenticated()) return Page();
        _repositoryManager.LikeRepository.AddLike(new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId });
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(Cheep cheep)
    {
        if (!IsUserAuthenticated()) return Page();
        _repositoryManager.LikeRepository.RemoveLike(new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId });
        return RedirectToPage();
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        _repositoryManager.FollowRepository.AddFollow(
            new Follow { FollowerId = GetAuthor().AuthorId, FollowedId = GetAuthor(routeName).AuthorId }
        );
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        _repositoryManager.FollowRepository.RemoveFollow(
            new Follow { FollowerId = GetAuthor().AuthorId, FollowedId = GetAuthor(routeName).AuthorId }
        );
        return RedirectToPage();
    }

    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        RouteName = HttpContext.GetRouteValue("author")?.ToString() ?? "";
        var isAuthor = CalculateIsAuthor(author, GetUserName());
        NumOfCheeps = _repositoryManager.CheepRepository.QueryCheepCount(author, isAuthor);

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
        Cheeps = _repositoryManager.CheepRepository.QueryCheeps(page, CheepsPerPage, author, isAuthor).ToList();
        return Page();
    }
}
