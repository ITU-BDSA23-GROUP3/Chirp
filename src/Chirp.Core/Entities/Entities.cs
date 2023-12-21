using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities;

/// <summary>
/// Represents an author in the Chirp application.
/// </summary>
public class Author
{
    /// <summary>
    /// The id and primary key of the author.
    /// </summary>
    [Key]
    public int AuthorId { get; set; }

    /// <summary>
    /// The name of the author.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The email of the author.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The cheeps authored by this author.
    /// </summary>
    public ICollection<Cheep>? Cheeps { get; set; }
}

/// <summary>
/// Represents a cheep in the Chirp application.
/// </summary>
public class Cheep
{
    /// <summary>
    /// The id and primary key of the cheep.
    /// </summary>
    [Key]
    public int CheepId { get; set; }

    /// <summary>
    /// The author id of the cheep.
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// The author of the cheep.
    /// </summary>
    public Author? Author { get; set; }

    /// <summary>
    /// The text content of the cheep.
    /// </summary>
    [MaxLength(160)]
    public required string Text { get; set; }

    /// <summary>
    /// The timestamp when the cheep was created.
    /// </summary>
    public required DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets a serialized representation of the cheep timestamp.
    /// </summary>
    /// <returns> A string representing the formatted timestamp. </returns>
    public string GetSerializedTimeStamp()
    {
        return $"{TimeStamp:MM/dd/yy H:mm:ss}";
    }

    /// <summary>
    /// Gets a string representation of the cheep.
    /// </summary>
    /// <returns> A string containing author name, cheep text, and timestamp. </returns>
    public override string ToString()
    {
        return Author?.Name + " " + Text + " " + GetSerializedTimeStamp();
    }
}

/// <summary>
/// Represents a "like" relationship in the Chirp application.
/// </summary>
public class Like
{
    /// <summary>
    /// The id of the author who performed the like.
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// The id of the cheep that was liked.
    /// </summary>
    public int CheepId { get; set; }
}

/// <summary>
/// Represents a "follow" relationship in the Chirp application.
/// </summary>
public class Follow
{
    /// <summary>
    /// The id of the follower.
    /// </summary>
    public int FollowerId { get; set; }

    /// <summary>
    /// The id of the user being followed.
    /// </summary>
    public int FollowedId { get; set; }
}
