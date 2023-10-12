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
        return _db.Cheeps.Count();
    }
    public int CountCheepsFromAuthor(string author)
    {
        return _db.Cheeps.Count(c => c.Author.Name == author);
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

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, int amount, string author)
    {
        int startIndex = pageNumber * amount;
        var cheeps = _db.Cheeps
            .Skip(startIndex)
            .Where(c => c.Author.Name == author)
            .Include(c => c.Author)
            .Take(amount)
            .ToList();

        return cheeps;
    }

    public IEnumerable<Cheep> GetCheepsPerPage(int pageNumber, int amount)
    {
        int startIndex = pageNumber * amount;

        var cheeps = _db.Cheeps
            .Skip(startIndex)
            .Include(c => c.Author)
            .Take(amount)
            .ToList();

        return cheeps;
    }
}
