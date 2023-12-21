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
    void StoreCheep(Cheep cheep);
        
    IQueryable<Cheep> GetAllCheepsByAuthorIds(List<int> authorIds);
    IQueryable<Cheep> GetAllCheeps();
    IQueryable<Cheep> GetAllCheepsByAuthorId(int authorId);

    /// <summary>
    ///  Gets paginated part of cheeps by multiple Authors.
    /// </summary>
    /// <param name="skip"> The amount of cheeps to skip. </param>
    /// <param name="take"> The amount of cheeps to take. </param>
    /// <param name="authorIds"> list of authors, to find cheeps they made. </param>
    /// <returns> A subset of cheeps, formatted for pagination. </returns>
    IEnumerable<Cheep> GetPaginatedCheepsByAuthorIds(List<int> authorIds, int skip, int take);

    /// <summary>
    /// Gets paginated part of all cheeps.
    /// </summary>
    /// <param name="skip"> The amount of cheeps to skip. </param>
    /// <param name="take"> The amount of cheeps to take. </param>
    /// <returns> A subset of cheeps, formatted for pagination. </returns>
 
    IEnumerable<Cheep> GetPaginatedCheeps(int skip, int take);

    /// <summary>
    /// Gets paginated part of cheeps by an Author.
    /// </summary>
    /// <param name="skip"> The amount of cheeps to skip. </param>
    /// <param name="take"> The amount of cheeps to take. </param>
    /// <param name="authorId"> id of author, to find cheeps they made. </param>
    /// <returns> A subset of cheeps, formatted for pagination. </returns>
 
    IEnumerable<Cheep> GetPaginatedCheepsByAuthorId(int authorId, int skip, int take);

    /// <summary>
    /// Deletes all cheeps that match the given Author.
    /// </summary>
    /// <param name="author"> Author whose cheeps should be removed. </param>
    void DeleteAllCheepsByAuthorId(int authorId);

     /// <summary>
    /// Deletes given Cheep from the database.
    /// </summary>
    /// <param name="cheep"> The Cheep to delete from the database. </param>
    void DeleteCheep(int cheepId);
    
}
