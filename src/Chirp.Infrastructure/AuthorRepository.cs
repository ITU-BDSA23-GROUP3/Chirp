using Chirp.Core;

namespace Chirp.Infrastructure;

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

    public void CreateAuthor(Author author)
    {
        var authorCheck = _db.Authors
            .Where(a => a.Name == author.Name && a.Email == author.Email);

        if (!authorCheck.Any())
        {   
            int newAuthorId = _db.Authors.Any() ? _db.Authors.Max(author => author.AuthorId) + 1 : 1;
            author.AuthorId = newAuthorId;
            _db.Authors.Add(author);
            _db.SaveChanges();
        }
    }
}
