using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private ChirpDBContext _db;
    protected IAuthorRepository _authorRepository;
    protected IFollowRepository _followRepository;

    public CheepRepository(ChirpDBContext db, IFollowRepository followRepository, IAuthorRepository authorRepository)
    {
        _db = db;
        _db.Database.EnsureCreated();
        _followRepository = followRepository;
        _authorRepository = authorRepository;
    }
    
    public void StoreCheep(Cheep entity)
    {
        StoreCheeps(new List<Cheep> { entity });
    }

    public void StoreCheeps(List<Cheep> entities)
    {
        _db.Cheeps.AddRange(entities);
        _db.SaveChanges();
    }

    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null)
    {
        int startIndex = (pageNumber -1) * amount;
        
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {
            var authorId = _authorRepository.CreateAuthor(author, "example@mail.com").AuthorId;
            var followedIds = _followRepository.FindFollowingByAuthorId(authorId).Select(f => f.FollowedId);
            queryResult = _db.Cheeps.Where(c => c.AuthorId == authorId);
        }

        return queryResult.OrderByDescending(c => c.TimeStamp).Skip(startIndex).Include(c => c.Author).Take(amount);
    }
        
    public int QueryCheepCount(string? author = null)
    {
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {
            var authorId = _authorRepository.CreateAuthor(author, "example@mail.com").AuthorId;
            var followedIds = _followRepository.FindFollowingByAuthorId(authorId).Select(f => f.FollowedId);
            queryResult = _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == authorId);
        }

        return queryResult.Count();
    }
}
