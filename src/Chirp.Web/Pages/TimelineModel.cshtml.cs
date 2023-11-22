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

    public async Task<IActionResult> OnPostAsync()
    {
        string text = Request.Form["Text"];
        if (text.Length > 160) text = text.Substring(0, 160);

        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _service.StoreCheep( new Cheep {AuthorId = authorId, Text=text, TimeStamp = DateTime.Now} );
        return RedirectToPage();
    }

    public bool AuthorLikesCheep(string name, int cheepId)
    {
        var authors = _authorRepository.FindAuthorsByName(name);
        if (!authors.Any()) return false; // Should never happen
        return _likeRepository.LikeExists(authors.First().AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(string authorName, int cheepId)
    {
        var authors = _authorRepository.FindAuthorsByName(authorName);
        if (!authors.Any()) return false;
        return _likeRepository.LikesOwnCheep(authors.First().AuthorId, cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public bool AuthorFollowsAuthor(string followerName, string followedName)
    {
        var followers = _authorRepository.FindAuthorsByName(followerName);
        if (!followers.Any()) return false; // Should never happen

        var followed = _authorRepository.FindAuthorsByName(followedName);
        if (!followed.Any()) return false; // Should never happen

        return _followRepository.FollowExists(followers.First().AuthorId, followed.First().AuthorId);
    }

    public int GetFollowersCount(string routeName)
    {
        var authors = _authorRepository.FindAuthorsByName(routeName);
        if (!authors.Any()) return 0;

        return _followRepository.FindFollowersCountByAuthorId(authors.First().AuthorId);
    }

    public int GetFollowingCount(string routeName)
    {
        var authors = _authorRepository.FindAuthorsByName(routeName);
        if (!authors.Any()) return 0;

        return _followRepository.FindFollowingCountByAuthorId(authors.First().AuthorId);
    }

    public IActionResult OnPostFollow(string routeName)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int followerId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        int followedId = _authorRepository.FindAuthorsByName(routeName).First().AuthorId;

        _followRepository.Follow(followerId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        int followerId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        int followedId = _authorRepository.FindAuthorsByName(routeName).First().AuthorId;

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

        var authorIsThemselves = author == User.Identity.Name;
        Cheeps = _service.GetCheeps(page, author, authorIsThemselves);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
