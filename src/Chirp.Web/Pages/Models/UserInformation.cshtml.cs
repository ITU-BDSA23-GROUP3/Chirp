
namespace Chirp.Web.Pages.Models;

public class UserInformationModel : ChirpModel
{
    public UserInformationModel(ICheepService service, IRepositoryManager repositoryManager)
        : base(service, repositoryManager) {}

    public IActionResult OnPostDelete()
    {
        var authorId = GetAuthor().AuthorId;

        // Delete all cheeps, likes and follows
        _repositoryManager.FollowRepository.DeleteAllFollowsByAuthorId(authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesByAuthorId(authorId);
        _repositoryManager.LikeRepository.DeleteAllLikesOnCheepsByAuthorId(authorId);
        _repositoryManager.AuthorRepository.DeleteAuthor(authorId);
        _service.DeleteCheeps(authorId);

        // Logout
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");

    }
}
