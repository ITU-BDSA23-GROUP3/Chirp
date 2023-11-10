namespace Chirp.Web.Pages;

public class ResetTables : PageModel
{

    private ChirpDBContext _db;
    public ResetTables(ChirpDBContext db)
    {
        _db = db;
    }
    public IActionResult OnPost()
    {
        _db.Cheeps.RemoveRange(_db.Cheeps);
        _db.Authors.RemoveRange(_db.Authors);
        _db.SaveChanges();

        DbInitializer.SeedDatabase(_db);

        return RedirectToPage("Public");
    }
}