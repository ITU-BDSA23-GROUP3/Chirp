using Chirp.Core;

namespace Chirp.Infrastructure;

/// <inheritdoc cref="ILikeRepository" />
public class LikeRepository : ILikeRepository
{
    private readonly ChirpDBContext _db;

    public LikeRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public bool LikesOwnCheep(int authorId, int cheepId)
    {
        var cheep = _db.Cheeps.Where(c => c.CheepId == cheepId).FirstOrDefault();
        if (cheep == null) return false;
        return cheep.AuthorId == authorId;
    }

    public void AddLike(Like like)
    {
        if (LikeExists(like)) return;

        _db.Likes.Add(like);
        _db.SaveChanges();
    }

    public void RemoveLike(int authorId, int cheepId)
    {
        var existingLike = _db.Likes
            .FirstOrDefault(l => l.AuthorId == authorId && l.CheepId == cheepId);

        // In case the like doesn't exist to begin with, do nothing
        if (existingLike == null) return;

        _db.Likes.Remove(existingLike);
        _db.SaveChanges();
    }

    public bool LikeExists(Like like)
    {
        return _db.Likes.Any(l => l.AuthorId == like.AuthorId && l.CheepId == like.CheepId);
    }

    public IEnumerable<Like> FindLikesByCheepId(int cheepId)
    {
        return _db.Likes.Where(l => l.CheepId == cheepId);
    }

    public List<Like> FindLikesByAuthorId(int authorId)
    {
        return _db.Likes.Where(l => l.AuthorId == authorId).ToList();
    }

    public int FindLikeCountByCheepId(int cheepId)
    {
        return _db.Likes.Count(l => l.CheepId == cheepId);
    }

    public void DeleteAllLikesByAuthorId(int authorId)
    {
        var likes = FindLikesByAuthorId(authorId);
        _db.Likes.RemoveRange(likes);
        _db.SaveChanges();
    }

    public void DeleteAllLikesOnCheepsByAuthorId(int authorId)
    {
        var likes = FindAllLikesOnCheepsByAuthorId(authorId);
        _db.Likes.RemoveRange(likes);
        _db.SaveChanges();
    }
    
    public IQueryable<Like> FindAllLikesOnCheepsByAuthorId(int authorId)
    {
        var cheepIdsOfCheepsCreatedByAuthor = _db.Cheeps
            .Where(c => c.AuthorId == authorId)
            .Select(c => c.CheepId);
        
        return _db.Likes.Where(l => cheepIdsOfCheepsCreatedByAuthor.Contains(l.CheepId));
    }
}
