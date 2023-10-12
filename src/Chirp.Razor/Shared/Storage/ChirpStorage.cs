using System.Data;
using Chirp.Razor.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chirp.Razor.Shared.Storage;

public class ChirpStorage : IChirpStorage
{
    private readonly IStoragePathHandler _ph;
    private ChirpDBContext _db;

    public ChirpStorage(IStoragePathHandler ph)
    {
        _ph = ph;
        Console.WriteLine("Creating database");
        CreateDB();
    }

    private void CreateDB()
    {
        Console.WriteLine($"Now creating database at: {_ph.ChirpDbPath}");
        _db = new ChirpDBContext();
        DbInitializer.SeedDatabase(_db);
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
