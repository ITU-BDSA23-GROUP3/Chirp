using System.Data;
using Chirp.Razor.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chirp.Razor.Shared.Storage;

public class ChirpStorage : IChirpStorage
{
    private ChirpDBContext _db;

    public ChirpStorage(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }
    
    public int CountCheeps()
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
        _db.AddRange(entities);
        _db.SaveChanges();
    }

    public List<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null)
    {
        int startIndex = pageNumber * amount;
        var queryResult = _db.Cheeps.Skip(startIndex);

        if (!string.IsNullOrEmpty(author))
        {
            queryResult = queryResult.Where(c => c.Author.Name == author);
        }

        return queryResult.Include(c => c.Author).Take(amount).ToList();
    }
}
