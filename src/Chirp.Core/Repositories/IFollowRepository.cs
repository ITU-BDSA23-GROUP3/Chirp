namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to follows
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Inserts given Follow into the database
    /// </summary>
    /// <param name="follow"> The Follow to insert into the database </param>
    public void AddFollow(Follow follow);

    /// <summary>
    /// Deletes given Follow into the database
    /// </summary>
    /// <param name="follow"> The Follow to delete from the database </param>
    public void RemoveFollow(Follow follow);

    /// <summary>
    /// Checks whether given Follow exists in the database
    /// </summary>
    /// <param name="follow"> The Follow to check exists </param>
    /// <returns> True if given Follow exists; otherwise returns false </returns>
    public bool FollowExists(Follow follow);

    /// <summary>
    /// Finds all Authors the given Author is following
    /// </summary>
    /// <param name="author"> The Author used to find all followed Authors </param>
    /// <returns> An enumerable containing all Authors the given Author is following </returns>
    public IEnumerable<Follow> FindFollowingByAuthor(Author author);

    /// <summary>
    /// Finds all Authors the given Author is followed by
    /// </summary>
    /// <param name="author"> The Author used to find all followers </param>
    /// <returns> An enumerable containing all Authors the given Author is followed by </returns>
    public IEnumerable<Follow> FindFollowersByAuthor(Author author);

    /// <summary>
    /// Finds the amount of Authors the given Author is following
    /// </summary>
    /// <param name="author"> The Author used to find the amount of followed Authors </param>
    /// <returns> The amount of Authors the given Author is following </returns>
    public int FindFollowingCountByAuthor(Author author);

    /// <summary>
    /// Finds the amount of Authors the given Author is followed by
    /// </summary>
    /// <param name="author"> The Author used to find the amount of followers </param>
    /// <returns> The amount of Authors the given Author is followed by </returns>
    public int FindFollowersCountByAuthor(Author author);

    /// <summary>
    /// Deletes all follows pointing from or to the given Author
    /// </summary>
    /// <param name="author"> The Author whose follows are deleted </param>
    public void DeleteAllFollowsByAuthor(Author author);
}
