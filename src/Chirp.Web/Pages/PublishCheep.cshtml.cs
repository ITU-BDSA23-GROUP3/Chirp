using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class PublishCheep : PageModel
{
    private readonly ICheepRepository _cheepRepository;

    private ChirpDBContext _db;

    private readonly IAuthorRepository _authorRepository;
    
    [BindProperty]
    public string? Text { get; set; }
    public PublishCheep(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _db = db;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public IActionResult OnGet()
    {
        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if(Text.Length > 180) Text = Text.Substring(0, 180);

        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");
        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        var newCheepId = _db.Cheeps.Max(cheep => cheep.CheepId) + 1;
        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = newCheepId, Text = Text, TimeStamp = DateTime.Now });
        return Page();
    }
}