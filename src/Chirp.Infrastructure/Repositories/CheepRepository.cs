using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

/// <inheritdoc cref="ICheepRepository" />
public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _db;
    private readonly IFollowRepository _followRepository;

    public CheepRepository(ChirpDBContext db, IFollowRepository followRepository)
    {
        _db = db;
        _followRepository = followRepository;
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

    public IQueryable<Cheep> GetQueryableCheeps(Author? author = null, bool isUser = false)
    {
        if (author == null)
        {
            return GetAll();
        }

        if (isUser)
        {
            return GetAllCheepsByAuthorAndFollowers(author);
        }
        
        return GetAllCheepsByAuthor(author);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthorAndFollowers(Author author) 
    {   
        var followedIds = _followRepository.FindFollowingByAuthor(author).Select(f => f.FollowedId);
        return _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.Author == author); // Complexity: O(n^2)
    }

    public IEnumerable<Cheep> GetCheepsPaginated(int skip, int take, IQueryable<Cheep>? cheepsToPaginate = null)
    {
        if (skip < 0 || take < 0)
        {
            throw new ArgumentException("Skip and take must be positive integers");
        }

        cheepsToPaginate ??= _db.Cheeps;

        return cheepsToPaginate.OrderByDescending(c => c.TimeStamp).Skip(skip).Include(c => c.Author).Take(take);
    }

    public IQueryable<Cheep> GetAll()
    {
        return _db.Cheeps.OrderByDescending(c => c.TimeStamp).Include(c => c.Author);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthor(Author author) 
    {
        return _db.Cheeps.Where(c => c.Author == author);
    }

    public void DeleteCheep(Cheep cheep) 
    {
        _db.Cheeps.Remove(cheep);
        _db.SaveChanges();
    }

    public void DeleteCheeps(IEnumerable<Cheep> cheeps) 
    {
        _db.Cheeps.RemoveRange(cheeps);
        _db.SaveChanges();
    }

    public void DeleteAllCheepsByAuthor(Author author)
    {
        var cheepsToDelete = GetAllCheepsByAuthor(author);
        DeleteCheeps(cheepsToDelete);
    }
}
