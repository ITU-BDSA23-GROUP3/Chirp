using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[Authorize]
public class Auth : PageModel
{
    public ActionResult OnGet(bool? signOut)
    {
        if (User.Identity.IsAuthenticated && signOut != null)
        {
            if (signOut.Value)
                Response.Cookies.Delete(".AspNetCore.Cookies");
        }
        return RedirectToPage("Public");
    }
}
