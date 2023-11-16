using Chirp.Core;

namespace Chirp.Infrastructure;

public class LikeRepository : ILikeRepository
{
    private ChirpDBContext _db;

    public LikeRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public bool LikesOwnCheep(int authorId, int cheepId)
    {
        var cheeps = _db.Cheeps.Where(c => c.CheepId == cheepId);
        if(!cheeps.Any()) return false;
        return cheeps.First().AuthorId == authorId;
    }

    public void LikeCheep(int authorId, int cheepId)
    {
        // In case the like already exists, do nothing
        if (LikeExists(authorId, cheepId)) return;

        // Check if cheep is owned by author
        if (LikesOwnCheep(authorId, cheepId))
        {
            throw new Exception("Liking your own cheeps is not allowed!");
        }

        _db.Likes.Add(new Like { AuthorId = authorId, CheepId = cheepId });
        _db.SaveChanges();
    }

    public void UnlikeCheep(int authorId, int cheepId)
    {
        var existingLike = _db.Likes
            .FirstOrDefault(l => l.AuthorId == authorId && l.CheepId == cheepId);

        // In case the like doesn't exist to begin with, do nothing
        if (existingLike == null) return;

        _db.Likes.Remove(existingLike);
        _db.SaveChanges();
    }

    public bool LikeExists(int authorId, int cheepId)
    {
        return _db.Likes.Any(l => l.AuthorId == authorId && l.CheepId == cheepId);
    }

    public IEnumerable<Like> FindLikesByCheepId(int cheepId)
    {
        return _db.Likes.Where(l => l.CheepId == cheepId);
    }

    public IEnumerable<Like> FindLikesByAuthorId(int authorId)
    {
        return _db.Likes.Where(l => l.AuthorId == authorId);
    }

    public int FindLikeCountByCheepId(int cheepId)
    {
        return _db.Likes.Count(l => l.CheepId == cheepId);
    }
}
