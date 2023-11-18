using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Chirp.Web.Pages;

[Authorize]
public class AuthModel : PageModel
{
    IAuthorRepository _authorRepository;
    UserData _userData;

    public AuthModel(IAuthorRepository authorRepository, UserData userData)
    {
        _authorRepository = authorRepository;
        _userData = userData;
    }

    public ActionResult OnGet(bool? signOut)
    {
        if (User?.Identity?.IsAuthenticated == true && signOut != null)
        {
            if (signOut.Value)
                Response.Cookies.Delete(".AspNetCore.Cookies");
        }
        var userName = User?.Identity?.Name;
        var userEmail = User?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        _userData.Author = _authorRepository.FindAuthorsByName(userName ?? "").FirstOrDefault();

        if (userName == null || userEmail == null) return RedirectToPage(); // vi skal finde en måde at håndtere dette

        _authorRepository.CreateAuthor(new Author { AuthorId = userId, Name = userName, Email = userEmail});
        return RedirectToPage("Public");
    }
}
