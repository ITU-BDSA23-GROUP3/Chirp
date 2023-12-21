using Chirp.Core;

namespace Chirp.Infrastructure;

/// <inheritdoc cref="IFollowRepository" />
public class FollowRepository : IFollowRepository
{
    private readonly ChirpDBContext _db;

    public FollowRepository(ChirpDBContext db)
    {
        _db = db;
    }

    public void AddFollow(Follow follow)
    {
        // In case the follow already exists, do nothing
        if (FollowExists(follow)) return;

        _db.Follows.Add(new Follow { FollowerId = follow.FollowerId, FollowedId = follow.FollowedId });
        _db.SaveChanges();
    }

    public void RemoveFollow(Follow follow)
    {
        var existingFollow = _db.Follows
            .FirstOrDefault(f => f.FollowerId == follow.FollowerId && f.FollowedId == follow.FollowedId);

        // In case the follow doesn't exist to begin with, do nothing
        if (existingFollow == null) return;

        _db.Follows.Remove(existingFollow);
        _db.SaveChanges();
    }

    public bool FollowExists(Follow follow)
    {
        return _db.Follows.Any(f => f.FollowerId == follow.FollowerId && f.FollowedId == follow.FollowedId);
    }

    public List<int> FindFollowingIds(int authorId)
    {
        return _db.Follows.Where(f => f.FollowerId == authorId)
            .Select(f => f.FollowedId)
            .ToList();
    }

    public List<int> FindFollowersIds(int authorId)
    {
        return _db.Follows.Where(f => f.FollowedId == authorId)
            .Select(f => f.FollowedId)
            .ToList();
    }

    public int FindFollowingCount(int authorId) {
        return _db.Follows.Count(f => f.FollowerId == authorId);
    }

    public int FindFollowersCount(int authorId) {
        return _db.Follows.Count(f => f.FollowedId == authorId);
    }

    public void DeleteAllFollowsRelatedToAuthorId(int authorId) {
        var follows = _db.Follows.Where(f => f.FollowerId == authorId || f.FollowedId == authorId);
        _db.Follows.RemoveRange(follows);
        _db.SaveChanges();
    }
}
