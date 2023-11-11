namespace Chirp.Web.Pages;

public class ResetTablesModel : PageModel
{

    private ChirpDBContext _db;
    public ResetTablesModel(ChirpDBContext db)
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
