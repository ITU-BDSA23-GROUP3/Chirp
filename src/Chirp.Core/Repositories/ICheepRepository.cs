namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to cheeps.
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Inserts given Cheep into the database.
    /// </summary>
    /// <param name="cheep"> The Cheep to insert into the database. </param>
    public void StoreCheep(Cheep cheep);

    /// <summary>
    /// Inserts given Cheeps into the database.
    /// </summary>
    /// <param name="entities"> The collection of Cheeps to insert into the database. </param>
    public void StoreCheeps(ICollection<Cheep> entities);

    /// <summary>
    /// Deletes given Cheep from the database.
    /// </summary>
    /// <param name="cheep"> The Cheep to delete from the database. </param>
    public void DeleteCheep(Cheep cheep);

    /// <summary>
    /// Deletes all cheeps that match the given Author.
    /// </summary>
    /// <param name="author"> Author whose cheeps should be removed. </param>
    public void DeleteAllCheepsByAuthor(Author author);

    public IEnumerable<Cheep> GetCheepsPaginated(int skip, int take, IQueryable<Cheep>? cheepsToPaginate = null);

    public IQueryable<Cheep> GetQueryableCheeps(Author? author = null, bool isUser = false);
}
