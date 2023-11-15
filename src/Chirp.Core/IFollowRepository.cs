namespace Chirp.Core;

public interface IFollowRepository
{
    public void Follow(int followerId, int followedId);
    public void Unfollow(int followerId, int followedId);
    public bool FollowExists(int followerId, int followedId);
    public IEnumerable<Follow> FindFollowingByAuthorId(int authorId);
    public IEnumerable<Follow> FindFollowersByAuthorId(int authorId);
    public int FindFollowingCountByAuthorId(int authorId);
    public int FindFollowersCountByAuthorId(int authorId);
}
