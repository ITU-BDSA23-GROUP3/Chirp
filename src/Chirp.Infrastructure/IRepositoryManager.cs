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
    private IAuthorRepository _authorRepository;
    private ICheepRepository _cheepRepository;
    private IFollowRepository _followRepository;
    private ILikeRepository _likeRepository;

    public RepositoryManager(ChirpDBContext db)
    {
        _db = db;
        _db.Database.EnsureCreated();
    }

    public IAuthorRepository AuthorRepository
    { 
        get 
        {
            return _authorRepository ??= new AuthorRepository(_db);
        }
    }

    public IFollowRepository FollowRepository 
    {
        get 
        {
            return _followRepository ??= new FollowRepository(_db);
        }
    }

    public ICheepRepository CheepRepository 
    {
        get 
        {
            return _cheepRepository ??= new CheepRepository(_db, FollowRepository, AuthorRepository);
        }
    }
    
    public ILikeRepository LikeRepository 
    {
        get
        {
            return _likeRepository ??= new LikeRepository(_db);
        }
    }
}
