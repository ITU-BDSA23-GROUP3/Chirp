using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <inheritdoc cref="ICheepRepository" />
public class CheepRepository : ICheepRepository
{
    private ChirpDBContext _db;
    protected IAuthorRepository _authorRepository;
    protected IFollowRepository _followRepository;

    public CheepRepository(ChirpDBContext db, IFollowRepository followRepository, IAuthorRepository authorRepository)
    {
        _db = db;
        _followRepository = followRepository;
        _authorRepository = authorRepository;
    }
    
    public void StoreCheep(Cheep cheep)
    {   
        var newCheepId = _db.Cheeps.Any() ? _db.Cheeps.Max(cheep => cheep.CheepId) + 1 : 1;
        StoreCheeps(new List<Cheep> { new Cheep { AuthorId= cheep.AuthorId, CheepId = newCheepId, Text = cheep.Text, TimeStamp = cheep.TimeStamp} });
    }

    public void StoreCheeps(ICollection<Cheep> entities)
    {
        _db.Cheeps.AddRange(entities);
        _db.SaveChanges();
    }

    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null, bool isAuthor = false)
    {
        int startIndex = (pageNumber -1) * amount;
        
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {

            var authors = _authorRepository.FindAuthorsByName(author);
            if(!authors.Any()) return new List<Cheep>();
            var foundAuthor = authors.First();
            IEnumerable<int> followedIds = new List<int>();
            if (isAuthor) {
                followedIds = _followRepository.FindFollowingByAuthor(foundAuthor).Select(f => f.FollowedId);
            }
            queryResult = _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == foundAuthor.AuthorId);
        }

        return queryResult.OrderByDescending(c => c.TimeStamp).Skip(startIndex).Include(c => c.Author).Take(amount);
    }
        
    public int QueryCheepCount(string? author = null, bool isAuthor = false)
    {
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {
            var authors = _authorRepository.FindAuthorsByName(author);
            if(!authors.Any()) return 0;
            var foundAuthor = authors.First();
            IEnumerable<int> followedIds = new List<int>();
            if (isAuthor) {
                followedIds = _followRepository.FindFollowingByAuthor(foundAuthor).Select(f => f.FollowedId);
            }
            queryResult = _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == foundAuthor.AuthorId);
        }

        return queryResult.Count();
    }

    public void DeleteCheep(Cheep cheep) 
    {
        _db.Cheeps.Remove(cheep);
        _db.SaveChanges();
    }

    public void DeleteAllCheepsByAuthorId(Author author) 
    {
        _db.Cheeps.Where(c => c.Author == author).ToList().ForEach(c => _db.Cheeps.Remove(c));
        _db.SaveChanges();
    }

}
