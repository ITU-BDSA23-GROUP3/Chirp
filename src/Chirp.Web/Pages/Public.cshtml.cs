
namespace Chirp.Web.Pages;

public class PublicModel : TimelineModel
{
    public PublicModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service) 
        : base(db, cheepRepository, authorRepository, service) {}

    public ActionResult OnGet([FromQuery] int page = 1)
    {
        NumOfCheeps = _service.GetCheepCount();
        int maxPage = (int)Math.Ceiling((double)NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if (page < 1 || page > maxPage)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page);

        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
