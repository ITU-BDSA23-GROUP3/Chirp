using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class PublishCheep : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private ChirpDBContext _db;

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

    [BindProperty]
    public string? Text { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        // Save the cheep message in your repository
        // You need to implement the logic to save it in your ICheepRepository
        ViewData["SubmittedCheep"] = Text;

        /*
        The problem is that since ids are not automatically created, we have to do cumbersome:
        - Check if user.identity.name exists as author
        - If no, add new author with id equal to amount of authors + 1 (bad)
        - Then store new cheep with the text and authorId of ^ (bad)

        This is the ugly solution, which seems to work:
        */

        var authorCheck = _db.Authors
            .Where(a => a.Name == User.Identity.Name);

        var numOfAuthors = _db.Authors.Count();

        if (authorCheck.Any() == false)
        {
            var author = new Author { AuthorId = numOfAuthors+1, Name = User.Identity.Name, Email = "placeholder@mail.com" };
            _db.Authors.Add(author);
            _db.SaveChanges();
        }

        var authorId = _db.Authors.Where(a => a.Name == User.Identity.Name).First().AuthorId;
        var cheepId = _db.Cheeps.Count() + 1;

        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = cheepId, Text = Text, TimeStamp = DateTime.Now });

        /*
        Instead, the above code should be contained in the repositories
        */

        return Page();
    }
}