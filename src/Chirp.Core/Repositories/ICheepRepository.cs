namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to cheeps
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Inserts given Cheep into the database
    /// </summary>
    /// <param name="cheep"> The Cheep to insert into the database </param>
    public void StoreCheep(Cheep cheep);

    /// <summary>
    /// Inserts given Cheeps into the database
    /// </summary>
    /// <param name="entities"> The collection of Cheeps to insert into the database </param>
    public void StoreCheeps(ICollection<Cheep> entities);

    /// <summary>
    /// Queries the database to find all Cheeps that match given arguments
    /// </summary>
    /// <param name="pageNumber"> The page number to display found Cheeps from </param>
    /// <param name="amount"> The amount of Cheeps to find </param>
    /// <param name="author"> Optional parameter that filters Cheeps by author name </param>
    /// <param name="isAuthor"> Optional parameter that includes Cheeps from followed Authors if true </param>
    /// <returns> A list of all matching cheeps </returns>
    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null, bool isAuthor = false);

    /// <summary>
    /// Queries the database to find the amount of cheeps that match given arguments
    /// </summary>
    /// <param name="author"> Optional parameter that filters Cheeps by author name </param>
    /// <param name="isAuthor"> Optional parameter that includes Cheeps from followed Authors if true </param>
    /// <returns> The amount of matching cheeps </returns>
    public int QueryCheepCount(string? author = null, bool isAuthor = false);

    /// <summary>
    /// Deletes given Cheep from the database
    /// </summary>
    /// <param name="cheep"> The Cheep to delete from the database </param>
    public void DeleteCheep(Cheep cheep);

    /// <summary>
    /// Deletes all cheeps that match the given Author
    /// </summary>
    /// <param name="author"> Author whose cheeps should be removed </param>
    public void DeleteAllCheepsByAuthorId(Author author);
}
