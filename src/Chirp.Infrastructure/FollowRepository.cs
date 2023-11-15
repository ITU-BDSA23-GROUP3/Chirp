using Chirp.Core;

namespace Chirp.Infrastructure;

public class FollowRepository : IFollowRepository
{
    private ChirpDBContext _db;

    public FollowRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public void Follow(int followerId, int followedId)
    {
        // In case the follow already exists, do nothing
        if (FollowExists(followerId, followedId)) return;

        _db.Follows.Add(new Follow { FollowerId = followerId, FollowedId = followedId });
        _db.SaveChanges();
    }

    public void Unfollow(int followerId, int followedId)
    {
        var existingFollow = _db.Follows
            .FirstOrDefault(f => f.FollowerId == followerId && f.FollowedId == followedId);

        // In case the follow doesn't exist to begin with, do nothing
        if (existingFollow == null) return;

        _db.Follows.Remove(existingFollow);
        _db.SaveChanges();
    }

    public bool FollowExists(int followerId, int followedId)
    {
        return _db.Follows.Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public IEnumerable<Follow> FindFollowingByAuthorId(int authorId)
    {
        return _db.Follows.Where(f => f.FollowerId == authorId);
    }

    public IEnumerable<Follow> FindFollowersByAuthorId(int authorId) {
        return _db.Follows.Where(f => f.FollowedId == authorId);
    }

    public int FindFollowingCountByAuthorId(int authorId) {
        return _db.Follows.Count(f => f.FollowerId == authorId);
    }

    public int FindFollowersCountByAuthorId(int authorId) {
        return _db.Follows.Count(f => f.FollowedId == authorId);
    }
}
