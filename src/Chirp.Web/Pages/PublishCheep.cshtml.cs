using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Web.Pages;

public class PublishCheep : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public PublishCheep(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
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

        // Missing validation for less than X characters (both client and server side)

        _authorRepository.CreateAuthor(User.Identity.Name, "example@mail.com");
        var authorId = _authorRepository.FindAuthorsByName(User.Identity.Name).First().AuthorId;
        var cheepId = _cheepRepository.QueryCheepCount() + 1;
        _cheepRepository.StoreCheep(new Cheep { AuthorId = authorId, CheepId = cheepId, Text = Text, TimeStamp = DateTime.Now });
        return Page();
    }
}