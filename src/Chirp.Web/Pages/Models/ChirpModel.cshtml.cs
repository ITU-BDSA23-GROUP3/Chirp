using Chirp.Core;
using Chirp.Core.Entities;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Models;

/// <summary>
/// Represents a base Razor Page model for common functionality used in Chirp application pages.
/// </summary>
public class ChirpModel : PageModel
{
    /// <summary>
    /// The repository manager providing access to various repositories.
    /// </summary>
    protected readonly IRepositoryManager RepositoryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChirpModel"/> class.
    /// </summary>
    /// <param name="repositoryManager"> The repository manager providing access to various repositories. </param>
    public ChirpModel(IRepositoryManager repositoryManager)
    {
        RepositoryManager = repositoryManager;
    }

    /// <summary>
    /// Gets the username of the authenticated user.
    /// </summary>
    /// <returns> The username of the authenticated user or null if not authenticated. </returns>
    public string? GetUserName()
    {
        return User?.Identity?.Name;
    }

    /// <summary>
    /// Gets the author based on the provided author name or the authenticated user's name.
    /// </summary>
    /// <param name="authorName"> The name of the author to retrieve. </param>
    /// <returns> The author entity. </returns>
    public Author GetAuthor(string? authorName = null)
    {
        authorName ??= GetUserName();
        if (authorName == null) throw new Exception("User is not authenticated!");

        var author = RepositoryManager.AuthorRepository.FindAuthorsByName(authorName);
        var enumerable = author.ToList();
        return !enumerable.Any() ? RepositoryManager.AuthorRepository.CreateAuthor(new Author{Email = "", Name = authorName}) : enumerable.First();
    }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <returns> True if the user is authenticated; otherwise, false. </returns>
    public bool IsUserAuthenticated()
    {
        return User.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets the authors that the current user is following.
    /// </summary>
    /// <returns> The collection of authors being followed by the current user. </returns>
    public IEnumerable<Author> GetFollowing()
    {
        var following = RepositoryManager.FollowRepository.FindFollowingByAuthor(GetAuthor());
        return RepositoryManager.AuthorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    /// <summary>
    /// Gets the authors who are following the current user.
    /// </summary>
    /// <returns> The collection of authors who are followers of the current user. </returns>
    public IEnumerable<Author> GetFollowers()
    {
        var followers = RepositoryManager.FollowRepository.FindFollowersByAuthor(GetAuthor());
        return RepositoryManager.AuthorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }
}
