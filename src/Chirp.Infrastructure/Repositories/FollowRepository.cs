using Chirp.Core.Entities;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Repositories;

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

    public IEnumerable<Follow> FindFollowingByAuthor(Author author)
    {
        return _db.Follows.Where(f => f.FollowerId == author.AuthorId);
    }

    public IEnumerable<Follow> FindFollowersByAuthor(Author author) {
        return _db.Follows.Where(f => f.FollowedId == author.AuthorId);
    }

    public int FindFollowingCountByAuthor(Author author) {
        return _db.Follows.Count(f => f.FollowerId == author.AuthorId);
    }

    public int FindFollowersCountByAuthor(Author author) {
        return _db.Follows.Count(f => f.FollowedId == author.AuthorId);
    }

    public void DeleteAllFollowsByAuthor(Author author) {
        var follows = _db.Follows.Where(f => f.FollowerId == author.AuthorId || f.FollowedId == author.AuthorId);
        _db.Follows.RemoveRange(follows);
        _db.SaveChanges();
    }
}
