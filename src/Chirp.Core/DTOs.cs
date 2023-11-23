namespace Chirp.Core
{
    public record AuthorDTO(int AuthorId, string Name, string Email);

    public record CreateAuthorDTO(string Name, string Email);

    public record CheepDTO(int CheepId, int AuthorId, string AuthorName, string Text, DateTime TimeStamp);

    public record CreateCheepDTO(int AuthorId, string Text, DateTime TimeStamp);
}
