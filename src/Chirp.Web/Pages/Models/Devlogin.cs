using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Chirp.Infrastructure;
using Chirp.Core;

namespace Chirp.Web.Pages.Models;
public class DevloginModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();
    IRepositoryManager _repositoryManager;
    private readonly IWebHostEnvironment _environment;

    public DevloginModel(IRepositoryManager repositoryManager, IWebHostEnvironment environment)
    {
        _repositoryManager = repositoryManager;
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        if (!_environment.IsDevelopment())
        {
            return RedirectToPage("Public");
        }
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!_environment.IsDevelopment())
        {
            return RedirectToPage("Public");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _repositoryManager.AuthorRepository.CreateAuthor(new Author{Name= Input.Username, Email= Input.Email});

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Username),
            };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal);

        return RedirectToPage("Public");
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Email { get; set; }
    }
}
