namespace Chirp.Web.Pages.Models;

public class TimelineModel : ChirpModel
{
    public string RouteName = "";
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    public int CurrentPage = 1;
    public int MaxCharacterCount = 160;
    public TimelineModel(ICheepService service, IRepositoryManager repositoryManager)
        : base(service, repositoryManager) {}

    public IActionResult OnPost()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text[..MaxCharacterCount];
        _service.StoreCheep( new Cheep {AuthorId = authorId, Text=text, TimeStamp = DateTime.Now} );
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
