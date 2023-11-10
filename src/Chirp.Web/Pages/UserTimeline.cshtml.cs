namespace Chirp.Web.Pages;

public class UserTimelineModel : TimelineModel
{
    public UserTimelineModel(ChirpDBContext db, ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService service) 
        : base(db, cheepRepository, authorRepository, service) {}

    public ActionResult OnGet(string author, [FromQuery] int page = 1)
    {
        _authorRepository.CreateAuthor(author, "example@mail.com");

        NumOfCheeps = _service.GetCheepCount(author);

        int maxPage = (int) Math.Ceiling((double) NumOfCheeps / _service.CheepsPerPage);

        if (page == 0)
        {
            page = 1;
        }

        if ((page < 1 || page > maxPage) && _cheepRepository.QueryCheepCount(author) != 0)
        {
            return RedirectToPage();
        }

        Cheeps = _service.GetCheeps(page, author);
        CheepsPerPage = _service.CheepsPerPage;
        return Page();
    }
}
