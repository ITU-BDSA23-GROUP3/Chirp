using Chirp.Razor.Storage.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<Cheep> Cheeps { get; set; } = new();

    public int CheepsPerPage;
    public int NumOfCheeps;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pageNumber = 1)
    {
        Cheeps = _service.GetCheeps(pageNumber);
        CheepsPerPage = _service.CheepsPerPage;
        NumOfCheeps = _service.GetNumOfCheeps();
        return Page();
    }
}
