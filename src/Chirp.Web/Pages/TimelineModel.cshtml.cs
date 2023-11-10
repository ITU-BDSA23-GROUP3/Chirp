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
    public TimelineModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service)
    {
        _db = db;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _service = service;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string text = Request.Form["Text"];
        if (text.Length > 180) text = text.Substring(0, 180);

        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");
        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        var newCheepId = _db.Cheeps.Max(cheep => cheep.CheepId) + 1;
        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = newCheepId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();

    }
}
