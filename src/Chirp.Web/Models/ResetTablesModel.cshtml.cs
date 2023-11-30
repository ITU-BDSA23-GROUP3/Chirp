namespace Chirp.Web.Models;

public class ResetTablesModel : PageModel
{

    private ChirpDBContext _db;
    public ResetTablesModel(ChirpDBContext db)
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
