namespace Chirp.Web.Storage;

public interface IAuthorRepository
{
    public IEnumerable<Author> FindAuthorsByName(string name);
    public IEnumerable<Author> FindAuthorsByEmail(string email);
    public void CreateAuthor(string name, string email);
    
}