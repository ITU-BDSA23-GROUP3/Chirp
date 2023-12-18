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
        _followRepository = followRepository;
        _authorRepository = authorRepository;
    }
    
    public void StoreCheep(Cheep cheep)
    {   
        var newCheepId = _db.Cheeps.Any() ? _db.Cheeps.Max(cheep => cheep.CheepId) + 1 : 1;
        StoreCheeps(new List<Cheep> { new Cheep { AuthorId= cheep.AuthorId, CheepId = newCheepId, Text = cheep.Text, TimeStamp = cheep.TimeStamp} });
    }

    public void StoreCheeps(List<Cheep> entities)
    {
        _db.Cheeps.AddRange(entities);
        _db.SaveChanges();
    }

    public IQueryable<Cheep> GetQueryableCheeps(string? author = null, bool isUser = false)
    {
        if (author == null)
        {
            return GetAll();
        }
        
        var authors = _authorRepository.FindAuthorsByName(author);
        if (!authors.Any()) return new List<Cheep>().AsQueryable(); // Exception: page of non-existent author

        var authorId = authors.First().AuthorId;

        if (isUser)
        {
            return GetAllCheepsByAuthorAndFollowers(authorId);
        }
        
        return GetAllCheepsByAuthorId(authorId);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthorAndFollowers(int authorId) 
    {   
        var followedIds = _followRepository.FindFollowingByAuthorId(authorId).Select(f => f.FollowedId);
        return _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == authorId); // Complexity: O(n^2)
    }

    private IEnumerable<Cheep> GetCheepsPaginated(int skip, int take, IQueryable<Cheep> cheepsToPaginate = null)
    {
        if (skip < 0 || take < 0)
        {
            throw new ArgumentException("Skip and take must be positive integers");
        }

        if (cheepsToPaginate == null)
        {
            cheepsToPaginate = _db.Cheeps;
        }

        return cheepsToPaginate.OrderByDescending(c => c.TimeStamp).Skip(skip).Include(c => c.Author).Take(take);
    }

    public IEnumerable<Cheep> GetCheepsPaginated(int pageNumber, int cheepsPerPage, string? author = null, bool isUser = false)
    {
        int skip = (pageNumber -1) * cheepsPerPage;
        var cheepsToBePaginated = GetQueryableCheeps(author, isUser);

        return GetCheepsPaginated(skip, cheepsPerPage, cheepsToBePaginated);
    }

    public IQueryable<Cheep> GetAll()
    {
        return _db.Cheeps.OrderByDescending(c => c.TimeStamp).Include(c => c.Author);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthorId(int authorId) 
    {
        return _db.Cheeps.Where(c => c.AuthorId == authorId);
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

    public void DeleteAllCheepsByAuthorId(int authorId)
    {
        var cheepsToDelete = GetAllCheepsByAuthorId(authorId);
        DeleteCheeps(cheepsToDelete);
    }

}
