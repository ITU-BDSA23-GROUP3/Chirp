namespace Chirp.Web.Pages;

public class TimelineModel : PageModel
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    protected readonly ICheepService _service;
    protected ChirpDBContext _db;
    protected readonly ICheepRepository _cheepRepository;
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ILikeRepository _likeRepository;
    protected readonly IFollowRepository _followRepository;
    public TimelineModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
    {
        _db = db;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string text = Request.Form["Text"];
        if (text.Length > 160) text = text.Substring(0, 160);

        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");
        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        var newCheepId = _db.Cheeps.Max(cheep => cheep.CheepId) + 1;
        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = newCheepId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }

    public bool AuthorLikesCheep(string name, int cheepId)
    {
        var authors = _authorRepository.FindAuthorsByName(name);
        if (!authors.Any()) throw new Exception("Author is not registered in Database!");
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

        // Ugly command needed since user is not automatically creates as author on login: github.com/ITU-BDSA23-GROUP3/Chirp/issues/129
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike(int cheepId)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        // Ugly command needed since user is not automatically creates as author on login: github.com/ITU-BDSA23-GROUP3/Chirp/issues/129
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public bool AuthorFollowsAuthor(string followerName, string followedName)
    {
        var followers = _authorRepository.FindAuthorsByName(followerName);
        if (!followers.Any()) throw new Exception("Follower of Author is not registered in Database!");

        var followed = _authorRepository.FindAuthorsByName(followedName);
        if (!followed.Any()) throw new Exception("Followed Author is not registered in Database!");

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

        // Ugly command needed since user is not automatically creates as author on login: github.com/ITU-BDSA23-GROUP3/Chirp/issues/129
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

        int followerId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        int followedId = _authorRepository.FindAuthorsByName(routeName).First().AuthorId;

        _followRepository.Follow(followerId, followedId);
        return RedirectToPage();
    }

    public IActionResult OnPostUnfollow(string routeName)
    {
        if (!User.Identity.IsAuthenticated) return Page();

        // Ugly command needed since user is not automatically creates as author on login: github.com/ITU-BDSA23-GROUP3/Chirp/issues/129
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

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

        if ((page < 1 || page > maxPage) && _cheepRepository.QueryCheepCount(author) != 0)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page, author);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}