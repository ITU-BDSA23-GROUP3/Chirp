namespace Chirp.Web.Pages.Models;

public class ResetTablesModel : ChirpModel
{
    private readonly ChirpDBContext _db;
    public ResetTablesModel(IRepositoryManager repositoryManager, ChirpDBContext db)
        : base(repositoryManager)
    {
        _db = db;
    }

    public IActionResult OnPost()
    {
        // Removes all pre-existing data from the database
        _db.Cheeps.RemoveRange(_db.Cheeps);
        _db.Authors.RemoveRange(_db.Authors);
        _db.Likes.RemoveRange(_db.Likes);
        _db.Follows.RemoveRange(_db.Follows);
        _db.SaveChanges();

        // Seeds the database with mock data
        DbInitializer.SeedDatabase(_db);

        // Removes user information, effectively logging out
        Response.Cookies.Delete(".AspNetCore.Cookies");

        return RedirectToPage("Public");
    }
}
