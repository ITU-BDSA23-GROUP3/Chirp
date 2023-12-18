using Chirp.Core;

namespace Chirp.Infrastructure;

/// <summary>
/// Represents a manager for accessing various repositories related to the Chirp application entities.
/// </summary>
public interface IRepositoryManager
{
    /// <summary>
    /// The repository for managing Author entities.
    /// </summary>
    IAuthorRepository AuthorRepository { get; }

    /// <summary>
    /// The repository for managing Cheep entities.
    /// </summary>
    ICheepRepository CheepRepository { get; }

    /// <summary>
    /// The repository for managing Follow entities.
    /// </summary>
    IFollowRepository FollowRepository { get; }

    /// <summary>
    /// The repository for managing Like entities.
    /// </summary>
    ILikeRepository LikeRepository { get; }
}

/// <inheritdoc cref="IRepositoryManager" />
public class RepositoryManager : IRepositoryManager
{
    private readonly ChirpDBContext _db;

    public IAuthorRepository AuthorRepository { get; }

    public ICheepRepository CheepRepository { get; }

    public IFollowRepository FollowRepository { get; }

    public ILikeRepository LikeRepository { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryManager"/> class with the specified Chirp database context.
    /// </summary>
    /// <param name="db">The Chirp database context.</param>
    public RepositoryManager(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();

        // Initialize repositories with the Chirp database context.
        AuthorRepository ??= new AuthorRepository(_db);
        FollowRepository ??= new FollowRepository(_db);
        CheepRepository ??= new CheepRepository(_db, FollowRepository);
        LikeRepository ??= new LikeRepository(_db);
    }
}
