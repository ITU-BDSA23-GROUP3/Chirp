using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    public int CheepId { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }

    [MaxLength(160)]
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }

    public string GetSerializedTimeStamp()
    {
        return $"{TimeStamp:MM/dd/yy H:mm:ss}";
    }

    public override string ToString()
    {
        return Author.Name + " " + Text + " " + TimeStamp;
    }
}

public class Like
{
    public int AuthorId { get; set; }
    public int CheepId { get; set; }
}

public class Follow
{
    public int FollowerId { get; set; }
    public int FollowedId { get; set; }
}