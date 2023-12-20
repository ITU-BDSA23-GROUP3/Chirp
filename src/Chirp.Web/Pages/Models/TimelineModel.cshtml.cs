using Chirp.Core.Entities;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Chirp.Web.Pages.Models;

/// <summary>
/// Represents a Razor Page model for handling user timelines and cheep-related actions.
/// </summary>
public class TimelineModel : ChirpModel
{
    /// <summary>
    /// The name of the current route.
    /// </summary>
    public string? RouteName = "";

    /// <summary>
    /// The list of cheeps displayed on the timeline.
    /// </summary>
    public List<Cheep> Cheeps { get; set; } = new ();

    /// <summary>
    /// The total number of cheeps.
    /// </summary>
    public int NumOfCheeps;

    /// <summary>
    /// The amount of cheeps to display per page.
    /// </summary>
    public uint CheepsPerPage = 32;

    /// <summary>
    /// The current page number.
    /// </summary>
    public int CurrentPage = 1;

    /// <summary>
    /// The maximum character count for cheep text.
    /// </summary>
    public int MaxCharacterCount = 160;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineModel"/> class.
    /// </summary>
    /// <param name="repositoryManager"> The repository manager providing access to various repositories. </param>
    public TimelineModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) { }

    /// <summary>
    /// Checks if the current page is the user's page or a public page.
    /// </summary>
    /// <param name="routeName"> The name of the route. </param>
    /// <returns> True if the page belongs to the authenticated user or is a public page; otherwise, false. </returns>
    public bool IsUserOrPublicPage(string routeName)
    {
        return routeName == GetUserName() || routeName == null;
    }

    /// <summary>
    /// Calculates if the specified page author is the same as the logged-in user.
    /// </summary>
    /// <param name="pageAuthor"> The name of the page author. </param>
    /// <param name="loggedInUser"> The name of the logged-in user. </param>
    /// <returns> True if the page author is the same as the logged-in user; otherwise, false. </returns>
    protected bool CalculateIsAuthor(string? pageAuthor, string? loggedInUser)
    {
        if (pageAuthor == null || loggedInUser == null) return false;
        return pageAuthor == loggedInUser;
    }

    /// <summary>
    /// Checks if the logged-in user follows the specified author.
    /// </summary>
    /// <param name="routeName">The name of the author.</param>
    /// <returns>True if the user follows the author; otherwise, false.</returns>
    public bool UserFollowsAuthor(string routeName)
    {
        return RepositoryManager.FollowRepository.FollowExists(
            new Follow { FollowerId = GetAuthor().AuthorId, FollowedId = GetAuthor(routeName).AuthorId }
        );
    }

    /// <summary>
    /// Checks if the logged-in user has liked the specified cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to check for likes. </param>
    /// <returns> True if the user likes the cheep; otherwise, false. </returns>
    public bool UserLikesCheep(Cheep cheep)
    {
        return RepositoryManager.LikeRepository.LikeExists(
            new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId }
        );
    }

    /// <summary>
    /// Gets the number of likes for the specified cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to retrieve like count for. </param>
    /// <returns> The number of likes for the cheep. </returns>
    public int GetLikeCount(Cheep cheep)
    {
        return RepositoryManager.LikeRepository.FindLikeCountByCheep(cheep);
    }

    /// <summary>
    /// Checks if the logged-in user likes their own cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to check for likes. </param>
    /// <returns> True if the user likes their own cheep; otherwise, false. </returns>
    public bool LikesOwnCheep(Cheep cheep)
    {
        return RepositoryManager.LikeRepository.LikesOwnCheep(
            new Like { AuthorId = GetAuthor().AuthorId, CheepId = cheep.CheepId }
        );
    }

    /// <summary>
    /// Gets the number of followers for the specified author.
    /// </summary>
    /// <param name="routeName"> The name of the author. </param>
    /// <returns> The number of followers for the author. </returns>
    public int GetFollowersCount(string routeName)
    {
        return RepositoryManager.FollowRepository.FindFollowersCountByAuthor(GetAuthor(routeName));
    }

    /// <summary>
    /// Gets the number of authors the specified author is following.
    /// </summary>
    /// <param name="routeName"> The name of the author. </param>
    /// <returns> The number of authors the author is following. </returns>
    public int GetFollowingCount(string routeName)
    {
        return RepositoryManager.FollowRepository.FindFollowingCountByAuthor(GetAuthor(routeName));
    }

    /// <summary>
    /// Handles the POST request for creating a new cheep.
    /// </summary>
    /// <returns> A redirect result to the same page after storing the new cheep. </returns>
    public IActionResult OnPost()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text[..MaxCharacterCount];
        RepositoryManager.CheepRepository.StoreCheep(new Cheep { AuthorId = authorId, Text = text, TimeStamp = DateTime.Now });
        return RedirectToPage();
    }

    /// <summary>
    /// Handles the POST request for liking a cheep.
    /// </summary>
    /// <param name="cheepId"> The id of the cheep to like. </param>
    /// <param name="routeName"> The author name of the current route. </param>
    /// <param name="pageNumber"> The current page number. </param>
    /// <returns> A redirect result to the same page with the current query parameters. </returns>
    public IActionResult OnPostLike(int cheepId, string routeName, string pageNumber)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        RepositoryManager.LikeRepository.AddLike(new Like { AuthorId = authorId, CheepId = cheepId });
        return Redirect("/" + routeName + "?page=" + pageNumber);
    }

    /// <summary>
    /// Handles the POST request for unliking a cheep.
    /// </summary>
    /// <param name="cheepId"> The id of the cheep to unlike. </param>
    /// <param name="routeName"> The author name of the current route. </param>
    /// <param name="pageNumber"> The current page number. </param>
    /// <returns> A redirect result to the same page with the current query parameters. </returns>
    public IActionResult OnPostUnlike(int cheepId, string routeName, string pageNumber)
    {
        if (!IsUserAuthenticated()) return Page();
        var authorId = GetAuthor().AuthorId;
        RepositoryManager.LikeRepository.RemoveLike(new Like { AuthorId = authorId, CheepId = cheepId });
        return Redirect("/" + routeName + "?page=" + pageNumber);
    }

    /// <summary>
    /// Handles the POST request for following an author.
    /// </summary>
    /// <param name="followedId"> The id of the author to follow. </param>
    /// <param name="routeName"> The author name of the current route. </param>
    /// <param name="pageNumber"> The current page number. </param>
    /// <returns> A redirect result to the same page with the current query parameters. </returns>
    public IActionResult OnPostFollow(int followedId, string routeName, string pageNumber)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        RepositoryManager.FollowRepository.AddFollow(new Follow { FollowerId = followerId, FollowedId = followedId });
        return Redirect("/" + routeName + "?page=" + pageNumber);
    }

    /// <summary>
    /// Handles the POST request for unfollowing an author.
    /// </summary>
    /// <param name="followedId"> The id of the author to unfollow. </param>
    /// <param name="routeName"> The author name of the current route. </param>
    /// <param name="pageNumber"> The current page number. </param>
    /// <returns> A redirect result to the same page with the current query parameters. </returns>
    public IActionResult OnPostUnfollow(int followedId, string routeName, string pageNumber)
    {
        if (!IsUserAuthenticated()) return Page();
        var followerId = GetAuthor().AuthorId;
        RepositoryManager.FollowRepository.RemoveFollow(new Follow { FollowerId = followerId, FollowedId = followedId });
        return Redirect("/" + routeName + "?page=" + pageNumber);
    }

    /// <summary>
    /// Handles the GET request for retrieving the timeline page.
    /// </summary>
    /// <param name="author"> The name of the author to display the timeline for. </param>
    /// <param name="page"> The page number for paginated results. </param>
    /// <returns> The timeline page result. </returns>
    public ActionResult OnGet(string? author, [FromQuery] int page = 1)
    {
        RouteName = HttpContext.GetRouteValue("author")?.ToString() ?? null;
        var isAuthor = CalculateIsAuthor(author, GetUserName());

        var authorEntity = RouteName.IsNullOrEmpty() ? null : GetAuthor(author);

        IEnumerable<int> followedIds = new List<int>();
        
        if (authorEntity != null)
        {
            followedIds = RepositoryManager.FollowRepository.FindFollowingByAuthor(authorEntity).Select(f => f.FollowedId);
        }
        var cheeps = RepositoryManager.CheepRepository.GetQueryableCheeps(followedIds, authorEntity, isAuthor);    
        
        NumOfCheeps = cheeps.Count();

        var maxPage = (int)Math.Ceiling((double)NumOfCheeps / CheepsPerPage);

        if ((page < 1 || page > maxPage) && NumOfCheeps != 0)
        {
            CurrentPage = 1;
            return RedirectToPage();
        }

        CurrentPage = page;

        if (page - 1 < 0 || CheepsPerPage < 0) return Page();
       
        Cheeps = RepositoryManager.CheepRepository.GetCheepsPaginated((uint)page - 1, CheepsPerPage, cheeps).ToList();
        return Page();
    }
}
