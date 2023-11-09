using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;

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

    public ActionResult OnGet([FromQuery] int page = 1)
    {
        NumOfCheeps = _service.GetCheepCount();
        int maxPage = (int) Math.Ceiling((double) NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if (page < 1 || page > maxPage)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page);

        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
