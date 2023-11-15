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
    public TimelineModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository)
    {
        _db = db;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
    }

    public bool AuthorLikesCheep(string name, int cheepId)
    {
        var authors = _authorRepository.FindAuthorsByName(name);
        if (!authors.Any()) return false;
        return _likeRepository.LikeExists(authors.First().AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId) {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public IActionResult OnPostLike(int cheepId)
    {
        if(!User.Identity.IsAuthenticated) return Page();

        // Ugly code still needed
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.LikeCheep(authorId, cheepId);
        return RedirectToPage();
    }

    public IActionResult OnPostRemoveLike(int cheepId)
    {
        if(!User.Identity.IsAuthenticated) return Page();

        // Ugly code still needed
        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");

        int authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        _likeRepository.UnlikeCheep(authorId, cheepId);
        return RedirectToPage();
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

    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        NumOfCheeps = _service.GetCheepCount(author);

        int maxPage = (int) Math.Ceiling((double) NumOfCheeps / _service.CheepsPerPage);

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
