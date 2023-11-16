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

    public int QueryCheepCount(string? author = null)
    {
        return string.IsNullOrEmpty(author) ?
            _db.Cheeps.Count() :
            _db.Cheeps.Count(c => c.Author.Name == author);
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
        int startIndex = (pageNumber - 1) * amount;
        IQueryable<Cheep> queryResult;

        if (!string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps.Where(c => c.Author.Name == author);

            _authorRepository.CreateAuthor(author, "example@mail.com");
            var followedIds = _followRepository.FindFollowingByAuthorId(_authorRepository.FindAuthorsByName(author).First().AuthorId).Select(f => f.FollowedId);

            foreach (int followedId in followedIds)
            {
                queryResult = queryResult.Union(_db.Cheeps.Where(c => c.AuthorId == followedId));
            }
        }
        else
        {
            queryResult = _db.Cheeps;
        }

        return queryResult.OrderByDescending(c => c.TimeStamp).Skip(startIndex).Include(c => c.Author).Take(amount);
    }
}
