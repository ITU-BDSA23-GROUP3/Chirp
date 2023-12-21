namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to follows.
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Inserts given Follow into the database.
    /// </summary>
    /// <param name="follow"> The Follow to insert into the database. </param>
    public void AddFollow(Follow follow);

    /// <summary>
    /// Deletes given Follow into the database.
    /// </summary>
    /// <param name="follow"> The Follow to delete from the database. </param>
    public void RemoveFollow(Follow follow);

    /// <summary>
    /// Checks whether given Follow exists in the database.
    /// </summary>
    /// <param name="follow"> The Follow to search for in database. </param>
    /// <returns> True if given Follow exists; otherwise returns false. </returns>
    public bool FollowExists(Follow follow);

    /// <summary>
    /// Finds all Authors the given Author is following.
    /// </summary>
    /// <param name="authorId"> The Author used to find all followed Authors. </param>
    /// <returns> An enumerable containing all Authors the given Author is following. </returns>
    public List<int> FindFollowingIds(int authorId);

    /// <summary>
    /// Finds all Authors the given Author is followed by.
    /// </summary>
    /// <param name="authorId"> The Author used to find all followers. </param>
    /// <returns> An enumerable containing all Authors the given Author is followed by. </returns>
    public List<int> FindFollowersIds(int authorId);

    /// <summary>
    /// Finds the amount of Authors the given Author is following.
    /// </summary>
    /// <param name="authorId"> The Author used to find the amount of followed Authors. </param>
    /// <returns> The amount of Authors the given Author is following. </returns>
    public int FindFollowingCount(int authorId);

    /// <summary>
    /// Finds the amount of Authors the given Author is followed by.
    /// </summary>
    /// <param name="authorId"> The Author used to find all followers. </param>
    /// <returns> The amount of Authors the given Author is followed by. </returns>
    public int FindFollowersCount(int authorId);

    /// <summary>
    /// Deletes all follows pointing from or to the given Author.
    /// </summary>
    /// <param name="authorId"> The Author whose follows are deleted. </param>
    public void DeleteAllFollowsRelatedToAuthorId(int authorId);
}
