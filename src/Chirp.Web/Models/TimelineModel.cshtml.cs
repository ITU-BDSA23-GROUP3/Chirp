namespace Chirp.Web.Models;

public class TimelineModel : ChirpModel
{
    public string RouteName = "";
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    public int CurrentPage = 1;
    public TimelineModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
        : base(authorRepository, service, likeRepository, followRepository) {}

    public IActionResult OnPost()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > 160) text = text.Substring(0, 160);
        _service.StoreCheep( new Cheep {AuthorId = authorId, Text=text, TimeStamp = DateTime.Now} );
        return RedirectToPage();
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        _likeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        _likeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(routeName).AuthorId;
        _followRepository.Follow(followerId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(routeName).AuthorId;
        _followRepository.Unfollow(followerId, followedId);
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
