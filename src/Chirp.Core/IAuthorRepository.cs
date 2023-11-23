namespace Chirp.Core;

public interface IAuthorRepository
{
    public IEnumerable<AuthorDTO> FindAuthorsByName(string name);
    public IEnumerable<AuthorDTO> FindAuthorsByEmail(string email);
    public void CreateAuthor(CreateAuthorDTO author);
    
}
