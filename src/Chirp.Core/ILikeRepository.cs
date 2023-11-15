namespace Chirp.Core;

public interface ILikeRepository
{
    public void LikeCheep(int authorId, int cheepId);
    public void UnlikeCheep(int authorId, int cheepId);
    public bool LikeExists(int authorId, int cheepId);
    public IEnumerable<Like> FindLikesByCheepId(int cheepId);
    public IEnumerable<Like> FindLikesByAuthorId(int authorId);
    public int FindLikeCountByCheepId(int cheepId);
}
