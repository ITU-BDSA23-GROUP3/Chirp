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

        var author = _db.Authors
            .Where(a => a.AuthorId == cheep.AuthorId)
            .SingleOrDefault();

        if (author == null) return;
        
        var newCheep = new Cheep { 
            AuthorId= cheep.AuthorId, 
            Author= author,
            CheepId = newCheepId, 
            Text = cheep.Text, 
            TimeStamp = cheep.TimeStamp
            };

        _db.Cheeps.Add(newCheep);
        _db.SaveChanges();
    }

    public IQueryable<Cheep> GetAllCheeps()
    {
        return _db.Cheeps
            .OrderByDescending(c => c.TimeStamp);
    }

    public IEnumerable<Cheep> GetPaginatedCheeps(int skip, int take)
    {
        return _db.Cheeps
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skip)
            .Include(c => c.Author)
            .Take(take);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthorId(int authorId) 
    {
        return _db.Cheeps
            .Where(c => c.AuthorId == authorId);
    }

    public IEnumerable<Cheep> GetPaginatedCheepsByAuthorId(int authorId, int skip, int take)
    {
        return _db.Cheeps
            .Where(c => c.AuthorId == authorId)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skip)
            .Include(c => c.Author)
            .Take(take);
    }

    public IQueryable<Cheep> GetAllCheepsByAuthorIds(List<int> authorIds)
    {
        return _db.Cheeps
            .OrderByDescending(c => c.TimeStamp)
            .Where(c => authorIds.Contains(c.AuthorId));
    }

    public IEnumerable<Cheep> GetPaginatedCheepsByAuthorIds(List<int> authorIds, int skip, int take)
    {
        return _db.Cheeps
            .Where(c => authorIds.Contains(c.AuthorId))
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skip)
            .Include(c => c.Author)
            .Take(take);
    }


    public Cheep? GetCheepById(int cheepId)
    {
        return _db.Cheeps
            .Where(c => c.CheepId == cheepId)
            .SingleOrDefault();
    }

    public void DeleteCheep(int cheepId) 
    {
        var cheep = _db.Cheeps
            .Where(c => c.CheepId == cheepId)
            .SingleOrDefault();
        
        _db.Cheeps.Remove(cheep);
        _db.SaveChanges();
    }

    public void DeleteCheeps(IQueryable<Cheep> cheeps) 
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
