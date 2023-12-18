namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to likes.
/// </summary>
public interface ILikeRepository
{
    /// <summary>
    /// Inserts given Like into the database.
    /// </summary>
    /// <param name="like"> The Like to insert into the database. </param>
    public void AddLike(Like like);

    /// <summary>
    /// Deletes given Like from the database.
    /// </summary>
    /// <param name="like"> The Like to delete from the database. </param>
    public void RemoveLike(Like like);

    /// <summary>
    /// Checks whether given Like exists in the database.
    /// </summary>
    /// <param name="like"> The Like to check exists. </param>
    /// <returns> True if given Like exists; otherwise returns false. </returns>
    public bool LikeExists(Like like);

    /// <summary>
    /// Finds all likes of given Cheep.
    /// </summary>
    /// <param name="cheep"> The Cheep from which to find all likes. </param>
    /// <returns> An enumerable containing all Likes of given Cheep. </returns>
    public IEnumerable<Like> FindLikesByCheep(Cheep cheep);

    /// <summary>
    /// Finds all likes of given Author.
    /// </summary>
    /// <param name="author"> The Author whose Likes to find. </param>
    /// <returns> An enumerable containing all Likes of given Author. </returns>
    public IEnumerable<Like> FindLikesByAuthor(Author author);

    /// <summary>
    /// Finds the amount of Likes of given Cheep.
    /// </summary>
    /// <param name="cheep"> The Cheep used to find the amount of Likes. </param>
    /// <returns> The amount of Likes the given Cheep has. </returns>
    public int FindLikeCountByCheep(Cheep cheep);

    /// <summary>
    /// Checks whether given Like is by the same Author as the Cheep Author.
    /// </summary>
    /// <param name="like"> The Like used to check if Author likes own Cheep. </param>
    /// <returns> True if Author likes own Cheep; otherwise returns false. </returns>
    public bool LikesOwnCheep(Like like);

    /// <summary>
    /// Deletes all likes of the given Author.
    /// </summary>
    /// <param name="author"> The Author whose likes to remove. </param>
    public void DeleteAllLikesByAuthor(Author author);

    /// <summary>
    /// Deletes all likes from Cheeps the given Author owns.
    /// </summary>
    /// <param name="author"> The Author whose Cheeps to remove Likes from. </param>
    public void DeleteAllLikesOnCheepsByAuthor(Author author);
}
