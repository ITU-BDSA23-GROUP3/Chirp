namespace Chirp.Core;

/// <summary>
/// Repository for database actions relating to authors.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Queries database to find all authors that match the given name.
    /// </summary>
    /// <param name="name"> The name to search for. </param>
    /// <returns> The Author that match the given name. </returns>
    public Author? FindAuthorByName(string name);

    /// <summary>
    /// Queries database to find the first found author that matches the given id.
    /// </summary>
    /// <param name="authorId"> The id to search for. </param>
    /// <returns> The author that is found first using the given id. </returns>
    /// <exception cref="InvalidOperationException"> Thrown in case no author is found. </exception>
    public Author FindAuthorById(int authorId);

    /// <summary>
    /// Queries database to find all authors that match the given ids.
    /// </summary>
    /// <param name="authorIds"> The list of ids to search for. </param>
    /// <returns> A list containing all authors that match the given ids. </returns>
    public IEnumerable<Author> FindAuthorsByIds(List<int> authorIds);

    /// <summary>
    /// Inserts given Author into the database.
    /// </summary>
    /// <param name="author"> The Author to insert into the database. </param>
    public void CreateAuthor(Author author);

    /// <summary>
    /// Deletes given Author from the database.
    /// </summary>
    /// <param name="author"> The Author to delete from the database. </param>
    public void DeleteAuthor(string authorName);
}
