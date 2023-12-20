using Chirp.Core.Entities;

namespace Chirp.Core.Repositories;

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

    /// <summary>
    /// Gets all cheeps from the database.
    /// </summary>
    /// <param name="followedIds">The ids of all the folllowers</param>
    /// <param name="author"> Optional parameter for only getting cheeps by the given author. </param>
    /// <param name="isUser"> Optional parameter for also getting cheeps by followed authors. </param>
    /// <returns> A queryable list of cheeps. </returns>
    public IQueryable<Cheep> GetQueryableCheeps(IEnumerable<int> followedIds, Author? author = null, bool isUser = false);

    /// <summary>
    /// Gets a subset of the available cheeps for pagination use.
    /// </summary>
    /// <param name="skip"> The amount of cheeps to skip. </param>
    /// <param name="take"> The amount of cheeps to take. </param>
    /// <param name="cheepsToPaginate"> Optional parameter for a list of cheeps to paginate. </param>
    /// <returns> A subset of cheeps, formatted for pagination. </returns>
    public IEnumerable<Cheep> GetCheepsPaginated(uint skip, uint take, IQueryable<Cheep>? cheepsToPaginate = null);

}
