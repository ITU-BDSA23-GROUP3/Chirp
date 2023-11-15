using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class LikeRepository : ILikeRepository
{
    private ChirpDBContext _db;

    public LikeRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public bool LikeExists(int authorId, int cheepId) {
        return _db.Likes.Any(l => l.AuthorId == authorId && l.CheepId == cheepId);
    }

    

    public void LikeCheep(int authorId, int cheepId)
    {
        if (LikeExists(authorId, cheepId)) return;

        _db.Likes.Add(new Like { AuthorId = authorId, CheepId = cheepId });
        _db.SaveChanges();
    }

    public void UnlikeCheep(int authorId, int cheepId)
    {
        var existingLike = _db.Likes
            .FirstOrDefault(l => l.AuthorId == authorId && l.CheepId == cheepId);
        
        if (existingLike == null) return;

        _db.Likes.Remove(existingLike);
        _db.SaveChanges();
    }

    public IEnumerable<Like> FindLikesByAuthorId(int authorId)
    {
        return _db.Likes.Where(l => l.AuthorId == authorId);
    }

    public IEnumerable<Like> FindLikesByCheepId(int cheepId)
    {
        return _db.Likes.Where(l => l.CheepId == cheepId).ToList();
    }

    public int FindLikeCountByCheepId(int cheepId)
    {
        return _db.Likes.Count(l => l.CheepId == cheepId);
    }

}
