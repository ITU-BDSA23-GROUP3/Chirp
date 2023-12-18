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

    public bool LikesOwnCheep(Like like)
    {
        var cheeps = _db.Cheeps.Where(c => c.CheepId == like.CheepId);
        if (!cheeps.Any()) return false;
        return cheeps.First().AuthorId == like.AuthorId;
    }

    public void AddLike(Like like)
    {
        // Check if like already exists
        if (LikeExists(like))
        {
            throw new Exception("Cheep is already liked by author!");
        }

        // Check if cheep is owned by author
        if (LikesOwnCheep(like))
        {
            throw new Exception("Liking your own cheeps is not allowed!");
        }

        _db.Likes.Add(like);
        _db.SaveChanges();
    }

    public void RemoveLike(Like like)
    {
        var existingLike = _db.Likes
            .FirstOrDefault(l => l.AuthorId == like.AuthorId && l.CheepId == like.CheepId);

        // In case the like doesn't exist to begin with, do nothing
        if (existingLike == null) return;

        _db.Likes.Remove(existingLike);
        _db.SaveChanges();
    }

    public bool LikeExists(Like like)
    {
        return _db.Likes.Any(l => l.AuthorId == like.AuthorId && l.CheepId == like.CheepId);
    }

    public IEnumerable<Like> FindLikesByCheep(Cheep cheep)
    {
        return _db.Likes.Where(l => l.CheepId == cheep.CheepId);
    }

    public IEnumerable<Like> FindLikesByAuthor(Author author)
    {
        return _db.Likes.Where(l => l.AuthorId == author.AuthorId);
    }

    public int FindLikeCountByCheep(Cheep cheep)
    {
        return _db.Likes.Count(l => l.CheepId == cheep.CheepId);
    }

    public void DeleteAllLikesByAuthor(Author author)
    {
        var likes = _db.Likes.Where(l => l.AuthorId == author.AuthorId);
        _db.Likes.RemoveRange(likes);
        _db.SaveChanges();
    }

    public void DeleteAllLikesOnCheepsByAuthor(Author author)
    {
        var cheeps = _db.Cheeps.Where(c => c.AuthorId == author.AuthorId);
        var cheepIds = cheeps.Select(c => c.CheepId);
        var likes = _db.Likes.Where(l => cheepIds.Contains(l.CheepId));
        _db.Likes.RemoveRange(likes);
        _db.SaveChanges();
    }
}
