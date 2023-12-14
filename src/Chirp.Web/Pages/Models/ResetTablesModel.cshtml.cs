using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages.Models;

/// <summary>
/// Represents a Razor Page model for resetting tables and seeding the database with mock data.
/// </summary>
public class ResetTablesModel : ChirpModel
{
    private readonly ChirpDBContext _db;
        private readonly IWebHostEnvironment _environment;


    /// <summary>
    /// Initializes a new instance of the <see cref="ResetTablesModel"/> class.
    /// </summary>
    /// <param name="repositoryManager"> The repository manager providing access to various repositories. </param>
    /// <param name="db"> The Chirp database context. </param>
    public ResetTablesModel(IRepositoryManager repositoryManager, ChirpDBContext db, IWebHostEnvironment environment)
        : base(repositoryManager)
    {
        _db = db;
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        // Page is inaccessible when not in development
        if(!_environment.IsDevelopment()) {
            return RedirectToPage("Public");
        }
        return Page();
    }

    /// <summary>
    /// Handles the POST request for resetting tables, removing existing data, and seeding the database with mock data.
    /// </summary>
    /// <returns> A redirect result to the "Public" page after resetting tables. </returns>
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

        // Redirects to the "Public" page after resetting tables
        return RedirectToPage("Public");
    }
}
