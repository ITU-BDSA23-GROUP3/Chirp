using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;

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

    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {

        NumOfCheeps = _service.GetCheepCount(author);

        int maxPage = (NumOfCheeps / _service.CheepsPerPage) + 1;

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
}
