namespace Chirp.Web.Pages.Models;

public class ChirpModel : PageModel
{
    protected readonly IRepositoryManager _repositoryManager;
    protected readonly ICheepService _service;

    public ChirpModel(ICheepService service, IRepositoryManager repositoryManager)
    {
        _service = service;
        _repositoryManager = repositoryManager;
    }

    public Author GetAuthor(string? authorName = null)
    {
        authorName ??= User?.Identity?.Name;
        if (authorName == null) throw new Exception("User is not authenticated!");
        
        var author = _repositoryManager.AuthorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author ?? throw new Exception("Author doesn't exist!");
    }

    public bool IsUserAuthenticated(){
        return User?.Identity != null && User.Identity.IsAuthenticated;
    }

    public bool UserLikesCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikeExists(GetAuthor().AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _repositoryManager.LikeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(int cheepId)
    {
        return _repositoryManager.LikeRepository.LikesOwnCheep(GetAuthor().AuthorId, cheepId);
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(followedName).AuthorId;
        return _repositoryManager.FollowRepository.FollowExists(followerId, followedId);
    }

    public IEnumerable<Author> GetFollowing(){
        var following = _repositoryManager.FollowRepository.FindFollowingByAuthorId(GetAuthor().AuthorId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    public IEnumerable<Author> GetFollowers(){
        var followers = _repositoryManager.FollowRepository.FindFollowersByAuthorId(GetAuthor().AuthorId);
        return _repositoryManager.AuthorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }

    public int GetFollowersCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowersCountByAuthorId(GetAuthor(routeName).AuthorId);
    }

    public int GetFollowingCount(string routeName)
    {
        return _repositoryManager.FollowRepository.FindFollowingCountByAuthorId(GetAuthor(routeName).AuthorId);
    }

    public bool IsUserOrPublicPage(string routeName) {
        return routeName == User.Identity?.Name || routeName == "";
    }

    protected bool CalculateIsAuthor(string? pageAuthor, string? loggedInUser) {
        if (pageAuthor == null|| loggedInUser == null) return false;
        return pageAuthor==loggedInUser;
    }

    public int GetCheepCount(string? authorName = null){
        return _service.GetCheepCount(authorName);
    }

    public IEnumerable<Cheep> GetCheeps(string? authorName = null){
        var user = GetAuthor(authorName);
        return _service.GetAllCheepsFromAuthor(user.Name);
    }

    public IEnumerable<Cheep> GetLikedCheeps(string? authorName = null){
        var likes = _repositoryManager.LikeRepository.FindLikesByAuthorId(GetAuthor(authorName).AuthorId);

        // Error-prone since it doesn't account for all pages (all cheeps)
        return _service.GetCheeps(1).Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }
}
