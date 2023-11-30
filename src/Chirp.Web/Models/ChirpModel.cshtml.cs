namespace Chirp.Web.Models;

public class ChirpModel : PageModel
{
    protected readonly ICheepService _service;
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ILikeRepository _likeRepository;
    protected readonly IFollowRepository _followRepository;
    public ChirpModel(IAuthorRepository authorRepository, ICheepService service, ILikeRepository likeRepository, IFollowRepository followRepository)
    {
        _authorRepository = authorRepository;
        _service = service;
        _likeRepository = likeRepository;
        _followRepository = followRepository;
    }

    public Author GetAuthor(string? authorName = null)
    {
        authorName ??= User?.Identity?.Name;
        if (authorName == null) throw new Exception("User is not authenticated!");
        
        var author = _authorRepository.FindAuthorsByName(authorName).FirstOrDefault();
        return author ?? throw new Exception("Author doesn't exist!");
    }

    public bool IsUserAuthenticated(){
        return User?.Identity != null && User.Identity.IsAuthenticated;
    }

    public bool UserLikesCheep(int cheepId)
    {
        return _likeRepository.LikeExists(GetAuthor().AuthorId, cheepId);
    }

    public int GetLikeCount(int cheepId)
    {
        return _likeRepository.FindLikeCountByCheepId(cheepId);
    }

    public bool LikesOwnCheep(int cheepId)
    {
        return _likeRepository.LikesOwnCheep(GetAuthor().AuthorId, cheepId);
    }

    public bool UserFollowsAuthor(string followedName)
    {
        var followerId = GetAuthor().AuthorId;
        var followedId = GetAuthor(followedName).AuthorId;
        return _followRepository.FollowExists(followerId, followedId);
    }

    public IEnumerable<Author> GetFollowing(){
        var following = _followRepository.FindFollowingByAuthorId(GetAuthor().AuthorId);
        return _authorRepository.FindAuthorsByIds(following.Select(f => f.FollowedId).ToList());
    }

    public IEnumerable<Author> GetFollowers(){
        var followers = _followRepository.FindFollowersByAuthorId(GetAuthor().AuthorId);
        return _authorRepository.FindAuthorsByIds(followers.Select(f => f.FollowerId).ToList());
    }

    public int GetFollowersCount(string routeName)
    {
        return _followRepository.FindFollowersCountByAuthorId(GetAuthor(routeName).AuthorId);
    }

    public int GetFollowingCount(string routeName)
    {
        return _followRepository.FindFollowingCountByAuthorId(GetAuthor(routeName).AuthorId);
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
        var likes = _likeRepository.FindLikesByAuthorId(GetAuthor(authorName).AuthorId);
        return _service.GetCheeps(1).Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }
}
