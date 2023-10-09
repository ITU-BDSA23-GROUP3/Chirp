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

        // Ensure that we don't overwrite the database
        if (!File.Exists(_ph.ChirpDbPath))
        {
            CreateDB();
        }
    }
    private void CreateDB()
    {
        Console.WriteLine($"Now creating database at: {_ph.ChirpDbPath}");
        _db = new ChirpDBContext();
        DbInitializer.SeedDatabase(_db);
    }
    public int CountCheeps()
    {
        return _db.Cheeps.Count();
    }
    public int CountCheepsFromAuthor(string author)
    {
        return _db.Cheeps.Where(c => c.Author.Name == author).Count();
    }
    
    public void StoreCheep(Cheep entity)
    {
        StoreCheeps(new List<Cheep>{entity});
    }

    public void StoreCheeps(List<Cheep> entities)
    {
    }

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, int amount, string author)
    {
        return _db.Cheeps.Where(c => c.Author.Name == author).ToList();
    }

    public IEnumerable<Cheep> GetCheepsPerPage(int pageNumber, int amount)
    {
        return new List<Cheep>();
    }
}
