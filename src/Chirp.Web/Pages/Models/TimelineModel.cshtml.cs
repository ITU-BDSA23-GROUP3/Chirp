using Chirp.Core;
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
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();

    /// <summary>
    /// The total number of cheeps.
    /// </summary>
    public int NumOfCheeps;

    /// <summary>
    /// The amount of cheeps to display per page.
    /// </summary>
    public int CheepsPerPage = 32;

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
        return routeName == GetUserName() || routeName == "";
    }

/// <summary>
    /// Calculates if the specified page author is the same as the logged-in user.
    /// </summary>
    /// <param name="pageAuthor"> The name of the page author. </param>
    /// <returns> True if the page author is the same as the logged-in user; otherwise, false. </returns>
    protected bool UsersPrivateTimeline(string? pageAuthor)
    {
        var loggedInUser = GetUserName();
        if (pageAuthor == null|| loggedInUser == null) return false;
        return pageAuthor==loggedInUser;
    }

    /// <summary>
    /// Checks if the logged-in user follows the specified author.
    /// </summary>
    /// <param name="routeName">The name of the author.</param>
    /// <returns>True if the user follows the author; otherwise, false.</returns>
    public bool UserFollowsAuthor(string routeName)
    {
        return _repositoryManager.FollowRepository.FollowExists(
            new Follow { FollowerId = GetAuthor().AuthorId, FollowedId = GetAuthor(routeName).AuthorId }
        );
    }

    /// <summary>
    /// Checks if the logged-in user has liked the specified cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to check for likes. </param>
    /// <returns> True if the user likes the cheep; otherwise, false. </returns>
    public bool UserLikesCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikeExists(new Like {AuthorId= GetAuthor().AuthorId, CheepId= cheepId});
    }

    /// <summary>
    /// Gets the number of likes for the specified cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to retrieve like count for. </param>
    /// <returns> The number of likes for the cheep. </returns>
    public int GetLikeCount(int cheepId)
    {
        return _repositoryManager.LikeRepository.FindLikeCountByCheepId(cheepId);
    }

    /// <summary>
    /// Checks if the logged-in user likes their own cheep.
    /// </summary>
    /// <param name="cheep"> The cheep to check for likes. </param>
    /// <returns> True if the user likes their own cheep; otherwise, false. </returns>
    public bool LikesOwnCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikesOwnCheep(GetAuthor().AuthorId, cheepId);
    }

    /// <summary>
    /// Gets the number of followers for the specified author.
    /// </summary>
    /// <param name="routeName"> The name of the author. </param>
    /// <returns> The number of followers for the author. </returns>
    public int GetFollowersCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowersCount(GetAuthor(routeName).AuthorId);
    }

    /// <summary>
    /// Gets the number of authors the specified author is following.
    /// </summary>
    /// <param name="routeName"> The name of the author. </param>
    /// <returns> The number of authors the author is following. </returns>
    public int GetFollowingCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowingCount(GetAuthor(routeName).AuthorId);
    }

     /// <summary>
    /// Handles the POST request for creating a new cheep.
    /// </summary>
    /// <returns> A redirect result to the same page after storing the new cheep. </returns>
    public IActionResult OnPostCheep()
    {
        var authorId = GetAuthor().AuthorId;
        string text = Request.Form["Text"].ToString();
        if (text.Length > MaxCharacterCount) text = text[..MaxCharacterCount];
        _repositoryManager.CheepRepository.StoreCheep(new Cheep{ AuthorId= authorId, Text= text, TimeStamp= DateTime.Now});
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
        _repositoryManager.LikeRepository.AddLike(new Like{AuthorId= authorId, CheepId= cheepId});
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
        _repositoryManager.LikeRepository.RemoveLike(authorId, cheepId);
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
        _repositoryManager.FollowRepository.AddFollow(new Follow { FollowerId = followerId, FollowedId = followedId });
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
        _repositoryManager.FollowRepository.RemoveFollow(new Follow { FollowerId = followerId, FollowedId = followedId });
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
        RouteName = HttpContext.GetRouteValue("author")?.ToString() ?? "";
        string timelineType = GetTimelineType(author);
        NumOfCheeps = GetTotalCheepCountForTimeline(timelineType, author);

        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / CheepsPerPage);

        if ((page < 1 || page > maxPage) && NumOfCheeps != 0)
        {
            page = 1;
        }

        var skip = (page - 1) * CheepsPerPage;

        CurrentPage = page;
        Cheeps = GetCheepsForTimeline(timelineType, author, skip, CheepsPerPage);
        return Page();
    }

    public string GetTimelineType(string? authorName)
    {
        // if public page
        if (authorName == null)
        {
            return "public page";
        }
        
        var author = GetAuthor(authorName);

        var isUser = UsersPrivateTimeline(authorName);
        // if current user's private page
        if (isUser)
        {
            return "private page";
        }

        // another users page
        return "user page";
    }

    public List<Cheep> GetCheepsForTimeline(string timelineType, string? authorName, int skip, int take)
    {
        if (timelineType == "public page")
        {
            return _repositoryManager.CheepRepository.GetPaginatedCheeps(skip, take).ToList();
        }

        var authorId = GetAuthor(authorName!).AuthorId;

        if (timelineType == "private page")
        {
            var authorIds = _repositoryManager.FollowRepository.FindFollowingIds(authorId).ToList();
            authorIds.Add(authorId);
            return _repositoryManager.CheepRepository.GetPaginatedCheepsByAuthorIds(authorIds, skip, take).ToList();
        }
        if (timelineType == "user page")
        {
            return _repositoryManager.CheepRepository.GetPaginatedCheepsByAuthorId(authorId, skip, take).ToList();
        }
        return null;
    }

    public int GetTotalCheepCountForTimeline(string timelineType, string? authorName)
    {
        if (timelineType == "public page")
        {
            return _repositoryManager.CheepRepository.GetAllCheeps().Count();
        }

        var authorId = GetAuthor(authorName!).AuthorId;

        if (timelineType == "private page")
        {
            var authorIds = _repositoryManager.FollowRepository.FindFollowingIds(authorId).ToList();
            authorIds.Add(authorId);
            return _repositoryManager.CheepRepository.GetAllCheepsByAuthorIds(authorIds).Count();
        }
        if (timelineType == "user page")
        {
            return _repositoryManager.CheepRepository.GetAllCheepsByAuthorId(authorId).Count();
        }

        return 0;
    }

}
