using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public int CheepsPerPage;
    public int NumOfCheeps;
    private ChirpDBContext _db;
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    public UserTimelineModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service)
    {
        _db = db;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {

        NumOfCheeps = _service.GetCheepCount(author);

        int maxPage = (int) Math.Ceiling((double) NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if (page < 1 || page > maxPage)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page, author);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }



    [BindProperty]
    public string? Text { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Text.Length > 180) Text = Text.Substring(0, 180);

        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");
        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        var newCheepId = _db.Cheeps.Max(cheep => cheep.CheepId) + 1;
        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = newCheepId, Text = Text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }
}
