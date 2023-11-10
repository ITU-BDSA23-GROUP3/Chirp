using Microsoft.AspNetCore.Authorization;

namespace Chirp.Web.Pages;

[Authorize]
public class AuthModel : PageModel
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
