using Chirp.Core;

namespace Chirp.Infrastructure;
public interface IRepositoryManager
{
    IAuthorRepository AuthorRepository { get; }
    ICheepRepository CheepRepository { get; }
    IFollowRepository FollowRepository { get; }
    ILikeRepository LikeRepository { get; }
}

public class RepositoryManager : IRepositoryManager
{
    private readonly ChirpDBContext _db;
    public IAuthorRepository AuthorRepository { get; }
    public ICheepRepository CheepRepository { get; }
    public IFollowRepository FollowRepository { get; }
    public ILikeRepository LikeRepository { get; }

    public RepositoryManager(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();

        AuthorRepository ??= new AuthorRepository(_db);
        FollowRepository ??= new FollowRepository(_db);
        CheepRepository ??= new CheepRepository(_db, FollowRepository, AuthorRepository);
        LikeRepository ??= new LikeRepository(_db);
    }
}