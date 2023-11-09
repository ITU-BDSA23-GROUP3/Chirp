using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private ChirpDBContext _db;

    public CheepRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
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
        int startIndex = (pageNumber -1) * amount;
        
        IQueryable<Cheep> queryResult;

        if (!string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps.Where(c => c.Author.Name == author);
        } else {
            queryResult = _db.Cheeps;
        }

        return queryResult.OrderByDescending(c => c.TimeStamp).Skip(startIndex).Include(c => c.Author).Take(amount);
    }
}
