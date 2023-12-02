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
            if (_authorRepository == null)
            {
                _authorRepository = new AuthorRepository(_db);
            }

            return _authorRepository;
        }
    }

    public ICheepRepository CheepRepository
    {
        get
        {
            if (_cheepRepository == null)
            {
                _cheepRepository = new CheepRepository(_db, FollowRepository, AuthorRepository);
            }

            return _cheepRepository;
        }
    }

    public IFollowRepository FollowRepository
    {
        get
        {
            if (_followRepository == null)
            {
                _followRepository = new FollowRepository(_db);
            }

            return _followRepository;
        }
    }

    public ILikeRepository LikeRepository
    {
        get
        {
            if (_likeRepository == null)
            {
                _likeRepository = new LikeRepository(_db);
            }

            return _likeRepository;
        }
    }
}