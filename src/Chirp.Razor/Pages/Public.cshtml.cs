using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public int cheepsPerPage;
    public int numOfCheeps;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pageNumber)
    {
        Cheeps = _service.GetCheeps(pageNumber);
        cheepsPerPage = _service.GetCheepsPerPage();
        numOfCheeps = _service.GetNumOfCheeps();
        return Page();
    }
}
