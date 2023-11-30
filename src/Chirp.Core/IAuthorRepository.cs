namespace Chirp.Core;

public interface IAuthorRepository
{
    public IEnumerable<Author> FindAuthorsByName(string name);
    public IEnumerable<Author> FindAuthorsByEmail(string email);
    public Author FindAuthorById(int authorId);
    public List<Author> FindAuthorsByIds(List<int> authorIds);
    public void CreateAuthor(Author author);
    
}
