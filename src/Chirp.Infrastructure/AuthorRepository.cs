using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Web.Storage;

namespace Chirp.Infrastructure.Storage;

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext _db;

    public AuthorRepository(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public IEnumerable<Author> FindAuthorsByName(string name)
    {
        var authorCheck = _db.Authors.Where(a => a.Name == name);
        return authorCheck;
    }

    public IEnumerable<Author> FindAuthorsByEmail(string email)
    {
        var authorCheck = _db.Authors.Where(a => a.Email == email);
        return authorCheck;
    }

    public void CreateAuthor(string name, string email)
    {
        var authorCheck = _db.Authors
            .Where(a => a.Name == name && a.Email == email);

        if (authorCheck.Any() == false){
            var author = new Author { Name = name, Email = email };
            _db.Authors.Add(author);
            _db.SaveChanges();
        }
    }
    
}
