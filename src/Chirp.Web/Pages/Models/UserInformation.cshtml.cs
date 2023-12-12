
namespace Chirp.Web.Pages.Models;

public class UserInformationModel : ChirpModel
{
    public UserInformationModel(IRepositoryManager repositoryManager)
        : base(repositoryManager) {}

    public IEnumerable<Cheep> GetCheeps(){
        // This can be cleaned up by moving the pagenumber parameter to the web project
        return _repositoryManager.CheepRepository.QueryCheeps(1, 10000, GetAuthor().Name, false).ToList();
    }

    public IEnumerable<Cheep> GetLikedCheeps(){
        var likes = _repositoryManager.LikeRepository.FindLikesByAuthor(GetAuthor());

        // Error-prone since it doesn't account for all pages (all cheeps)
        return _repositoryManager.CheepRepository.QueryCheeps(1, CheepsPerPage).ToList().Where(c => likes.Any(l => l.CheepId == c.CheepId));
    }

    public IActionResult OnPostDelete()
    {
        var author = GetAuthor();

        // Delete all cheeps, likes and follows
        _repositoryManager.FollowRepository.DeleteAllFollowsByAuthor(author);
        _repositoryManager.LikeRepository.DeleteAllLikesByAuthor(author);
        _repositoryManager.LikeRepository.DeleteAllLikesOnCheepsByAuthor(author);
        _repositoryManager.AuthorRepository.DeleteAuthor(author);
        _repositoryManager.CheepRepository.DeleteAllCheepsByAuthorId(author);

        // Logout
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToPage("Public");

    }
}
