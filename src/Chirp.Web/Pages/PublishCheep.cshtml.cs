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
                // Save the cheep message in your repository
                // You need to implement the logic to save it in your ICheepRepository
                ViewData["SubmittedCheep"] = Text;

                _authorRepository.CreateAuthor($"{User.Identity.Name}", $"wow");
                _cheepRepository.StoreCheep(new Cheep {AuthorId = 0, CheepId = 10002, Text = "Wow, added author, kinda!", TimeStamp = DateTime.Now});

                // Redirect to a different page, assuming "Public" is the correct destination
                return Page();
    }
}