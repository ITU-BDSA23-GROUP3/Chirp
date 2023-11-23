using System.Reflection.Metadata.Ecma335;
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

    public IEnumerable<AuthorDTO> FindAuthorsByName(string name)
    {
        var authorsQuery = _db.Authors.Where(a => a.Name == name);
        var authors = new List<AuthorDTO>();
        foreach (Author author in authorsQuery){
            authors.Add(new AuthorDTO(AuthorId: author.AuthorId, Name: author.Name, Email: author.Email));
        }
        return authors;
    }

    public IEnumerable<AuthorDTO> FindAuthorsByEmail(string email)
    {
        var authorsQuery = _db.Authors.Where(a => a.Email == email);
        var authors = new List<AuthorDTO>();
        foreach (Author author in authorsQuery){
            authors.Add(new AuthorDTO(AuthorId: author.AuthorId, Name: author.Name, Email: author.Email));
        }
        return authors;
    }

    public void CreateAuthor(CreateAuthorDTO author)
    {
        var authorCheck = _db.Authors
            .Where(a => a.Name == author.Name && a.Email == author.Email);

        if (!authorCheck.Any())
        {   
            var authorId = _db.Authors.Any() ? _db.Authors.Max(author => author.AuthorId) + 1 : 1;
            _db.Authors.Add(new Author 
            {
                AuthorId = authorId,
                Name = author.Name,
                Email = author.Email
            });
            _db.SaveChanges();
        }
    }
}
