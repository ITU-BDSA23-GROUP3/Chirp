using Chirp.Core;
using Chirp.Core.Entities;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

/// <inheritdoc cref="ICheepRepository" />
public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _db;

    public CheepRepository(ChirpDBContext db)
    {
        _db = db;
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

    public IQueryable<Cheep> GetQueryableCheeps(IEnumerable<int> followedIds, Author? author = null, bool isUser = false)
    {
        if (author == null)
        {
            return GetAll();
        }

        return isUser ? GetAllCheepsByAuthorAndFollowers(author, followedIds) : GetAllCheepsByAuthor(author);
    }

    private IQueryable<Cheep> GetAllCheepsByAuthorAndFollowers(Author author, IEnumerable<int> followedIds)
    {   
        return _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.Author == author); // Complexity: O(n^2)
    }

    public IEnumerable<Cheep> GetCheepsPaginated(uint skip, uint take, IQueryable<Cheep>? cheepsToPaginate = null)
    {
        cheepsToPaginate ??= _db.Cheeps;
        return cheepsToPaginate.OrderByDescending(c => c.TimeStamp).Skip((int)skip).Include(c => c.Author).Take((int)take);
    }

    private IQueryable<Cheep> GetAll()
    {
        return _db.Cheeps.OrderByDescending(c => c.TimeStamp).Include(c => c.Author);
    }

    private IQueryable<Cheep> GetAllCheepsByAuthor(Author author) 
    {
        return _db.Cheeps.Where(c => c.Author == author);
    }

    public void DeleteCheep(Cheep cheep) 
    {
        _db.Cheeps.Remove(cheep);
        _db.SaveChanges();
    }

    private void DeleteCheeps(IEnumerable<Cheep> cheeps) 
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
