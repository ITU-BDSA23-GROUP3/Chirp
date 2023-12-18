using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chirp.Infrastructure;
using Chirp.Core;

namespace Chirp.Web.Pages.Models;

/// <summary>
/// Represents a Razor Page model for handling authentication-related actions.
/// </summary>
[Authorize]
public class AuthModel : ChirpModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthModel"/> class.
    /// </summary>
    /// <param name="repositoryManager"> The repository manager providing access to various repositories. </param>
    public AuthModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) { }

    /// <summary>
    /// Handles the GET request for authentication-related actions.
    /// </summary>
    /// <param name="signOut"> A boolean indicating whether the user is signing out. </param>
    /// <returns> A redirect result to the "Public" page if successful. </returns>
    public ActionResult OnGet(bool? signOut)
    {
        // Check if the user is authenticated and the signOut parameter is provided.
        if (IsUserAuthenticated() && signOut != null && signOut.Value)
        {
            // If signing out, delete the authentication cookie
            Response.Cookies.Delete(".AspNetCore.Cookies");
            return RedirectToPage("Public");
        }

        // Retrieve user information from claims
        var userName = GetUserName();
        var userEmail = User?.FindFirst(ClaimTypes.Email)?.Value;

        // Throw an exception if user information is not available
        if (userName == null || userEmail == null)
        {
            throw new Exception("Could not find username/user email!");
        }

        // Create a new Author entity based on the authenticated user's information
        _repositoryManager.AuthorRepository.CreateAuthor(new Author { Name = userName, Email = userEmail });

        // Redirect to the "Public" page
        return RedirectToPage("Public");
    }
}
