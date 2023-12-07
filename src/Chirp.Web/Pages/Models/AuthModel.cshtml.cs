using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Web.Pages.Models;

[Authorize]
public class AuthModel : ChirpModel
{
    public AuthModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) {}

    public ActionResult OnGet(bool? signOut)
    {
        if (IsUserAuthenticated() && signOut != null)
        {
            if (signOut.Value)
                Response.Cookies.Delete(".AspNetCore.Cookies");
        }
        var userName = GetUserName();
        var userEmail = User?.FindFirst(ClaimTypes.Email)?.Value;

        if (userName == null || userEmail == null) throw new Exception("Could not find username/user email!");
    
        _repositoryManager.AuthorRepository.CreateAuthor(new Author { Name = userName, Email = userEmail});
        return RedirectToPage("Public");
    }
}
