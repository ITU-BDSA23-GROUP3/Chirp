
namespace Chirp.Web.Pages.Models;

public class UserInformationModel : ChirpModel
{
    public UserInformationModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) {}

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
