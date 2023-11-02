using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class Auth : PageModel
{
    public ActionResult OnGet()
    {
        return RedirectToPage("Public");
    }
}