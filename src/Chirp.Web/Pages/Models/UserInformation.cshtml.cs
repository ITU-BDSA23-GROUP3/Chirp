using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages.Models;

/// <summary>
/// Represents a Razor Page model for managing user information, cheeps, and related actions.
/// </summary>
public class UserInformationModel : ChirpModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserInformationModel"/> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager providing access to various repositories.</param>
    public UserInformationModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) { }

    /// <summary>
    /// Gets the cheeps authored by the authorized user.
    /// </summary>
    /// <returns> The cheeps authored by the authorized user. </returns>
    public IEnumerable<Cheep> GetCheepsByUser(){
        return _repositoryManager.CheepRepository.GetQueryableCheeps(GetAuthor()).ToList();
    }

    /// <summary>
    /// Gets the cheeps liked by the user.
    /// </summary>
    /// <returns> The cheeps liked by the user. </returns>
    public IEnumerable<Cheep> GetLikedCheeps()
    {
        var likes = _repositoryManager.LikeRepository.FindLikesByAuthor(GetAuthor());
        return _repositoryManager.CheepRepository.GetQueryableCheeps().Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }

    /// <summary>
    /// Handles the POST request for deleting user information, including user author, cheeps, likes, and follows.
    /// </summary>
    /// <returns> A redirect result to the "Public" page after deleting user information and logging out. </returns>
    public IActionResult OnPostDelete()
    {
        var author = GetAuthor();

        // Delete all cheeps, likes, and follows
        _repositoryManager.FollowRepository.DeleteAllFollowsByAuthor(author);
        _repositoryManager.LikeRepository.DeleteAllLikesByAuthor(author);
        _repositoryManager.LikeRepository.DeleteAllLikesOnCheepsByAuthor(author);
        _repositoryManager.AuthorRepository.DeleteAuthor(author);
        _repositoryManager.CheepRepository.DeleteAllCheepsByAuthor(author);

        // Logout
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");
    }
}
