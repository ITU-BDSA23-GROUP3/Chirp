namespace Chirp.Core;

public interface ILikeRepository
{
    public bool LikeExists(int authorId, int cheepId);
    public void LikeCheep(int authorId, int cheepId);
    public void UnlikeCheep(int authorId, int cheepId);
    public IEnumerable<Like> FindLikesByAuthorId(int authorId);
    public IEnumerable<Like> FindLikesByCheepId(int cheepId);
    public int FindLikeCountByCheepId(int cheepId);
}
