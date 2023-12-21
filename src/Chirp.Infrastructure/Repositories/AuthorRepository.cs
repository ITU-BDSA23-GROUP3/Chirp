using Chirp.Core;

namespace Chirp.Infrastructure;

/// <inheritdoc cref="IAuthorRepository" />
public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _db;

    public AuthorRepository(ChirpDBContext db)
    {
        _db = db;
    }

    public Author FindAuthorByName(string name)
    {
        return _db.Authors
            .Where(a => a.Name == name)
            .FirstOrDefault();
    }

    public Author FindAuthorById(int authorId)
    {
        return _db.Authors
            .Where(a => a.AuthorId == authorId)
            .FirstOrDefault();
    }

    public IEnumerable<Author> FindAuthorsByIds(List<int> authorIds)
    {
        return _db.Authors
            .Where(a => authorIds.Contains(a.AuthorId));
    }

    public void CreateAuthor(Author author)
    {
        var authorCheck = _db.Authors
            .Where(a => a.Name == author.Name && a.Email == author.Email);

        if (!authorCheck.Any())
        {   
            int newAuthorId = _db.Authors.Any() ? _db.Authors.Max(author => author.AuthorId) + 1 : 1;
            
            var newAuthor = new Author { 
                AuthorId = newAuthorId, 
                Name = author.Name, 
                Email = author.Email,
                Cheeps = new List<Cheep>()
            };

            _db.Authors.Add(newAuthor);
            _db.SaveChanges();
        }
    }

    public void DeleteAuthor(string authorName)
    {
        var author = FindAuthorByName(authorName);
            
        if (author != null) return;
        _db.Authors.Remove(author);
        _db.SaveChanges();
    }
}
