
namespace Chirp.Web.Pages.Models;

public class UserInformationModel : ChirpModel
{
    public UserInformationModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) {}

    public IEnumerable<Cheep> GetCheeps(string? authorName = null){
        // This can be cleaned up by moving the pagenumber parameter to the web project
        return _repositoryManager.CheepRepository.GetQueryableCheeps(GetAuthor(authorName).Name).ToList();
    }

    public IEnumerable<Cheep> GetLikedCheeps(string? authorName = null){
        var likes = _repositoryManager.LikeRepository.FindLikesByAuthorId(GetAuthor(authorName).AuthorId);

        return _repositoryManager.CheepRepository.GetQueryableCheeps(authorName).Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }

    public IActionResult OnPostDelete()
    {
        var authorId = GetAuthor().AuthorId;

        // Delete all cheeps, likes and follows
        _repositoryManager.FollowRepository.DeleteAllFollowsByAuthorId(authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesByAuthorId(authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesOnCheepsByAuthorId(authorId);
        _repositoryManager.AuthorRepository.DeleteAuthor(authorId);
        _repositoryManager.CheepRepository.DeleteAllCheepsByAuthorId(authorId);

        // Logout
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");

    }
}
