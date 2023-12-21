using Chirp.Core.Entities;
using Chirp.Core.Repositories;

namespace Chirp.Infrastructure.Repositories;

/// <inheritdoc cref="IAuthorRepository" />
public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _db;

    public AuthorRepository(ChirpDBContext db)
    {
        _db = db;
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

    public Author FindAuthorById(int authorId)
    {
        var authorCheck = _db.Authors.Where(a => a.AuthorId == authorId);
        return authorCheck.FirstOrDefault() ?? throw new InvalidOperationException("Could not find author by id!");
    }

    public List<Author> FindAuthorsByIds(List<int> authorIds)
    {
        var authorCheck = _db.Authors.Where(a => authorIds.Contains(a.AuthorId));
        return authorCheck.ToList();
    }

    public Author CreateAuthor(Author author)
    {
        var authorCheck = _db.Authors
            .Where(a => a.Name == author.Name && a.Email == author.Email);

        if (authorCheck.Any()) return authorCheck.First();
        
        var newAuthorId = _db.Authors.Any() ? _db.Authors.Max(a => a.AuthorId) + 1 : 1;
        author.AuthorId = newAuthorId;
        _db.Authors.Add(author);
        _db.SaveChanges();
        return author;
    }

    public void DeleteAuthor(Author author)
    {
        _db.Authors.Remove(author);
        _db.SaveChanges();
    }
}
