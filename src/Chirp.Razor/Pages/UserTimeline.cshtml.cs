using Chirp.Razor.Storage.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<Cheep> Cheeps { get; set; }
    public int CheepsPerPage;
    public int NumOfCheeps;


    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        Cheeps = _service.GetCheepsFromAuthor(page, author);
        CheepsPerPage = _service.CheepsPerPage;
        NumOfCheeps = _service.GetCheepCountFromAuthor(page, author);
        return Page();
    }
}
