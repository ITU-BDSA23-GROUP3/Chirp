using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpRepository : IChirpRepository
{
    private ChirpDBContext _db;

    public ChirpRepository(ChirpDBContext db)
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
        if(entity.AuthorId == 0)
        {
            throw new DbUpdateException("AuthorId must be set");
        }
        StoreCheeps(new List<Cheep> { entity });
    }

    public void StoreCheeps(List<Cheep> entities)
    {
        _db.Cheeps.AddRange(entities);
        _db.SaveChanges();
    }

    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null)
    {
        int startIndex = pageNumber * amount;
        var queryResult = _db.Cheeps.Skip(startIndex);

        if (!string.IsNullOrEmpty(author))
        {
            queryResult = queryResult.Where(c => c.Author.Name == author);
        }

        return queryResult.Include(c => c.Author).Take(amount);
    }
}
