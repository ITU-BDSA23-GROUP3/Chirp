
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var dbPath = StoragePathHandler.getPathToLocalFolder();

// Enable this code by setting the property on run, build, publish, etc.
// E.g., dotnet run -p:DefineConstants=SESSION_COOKIE_SUPPORT

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services
    .AddDbContext<ChirpDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
    .AddScoped<IChirpRepository, ChirpRepository>()
    .AddScoped<IAuthorRepository, AuthorRepository>()
    .AddScoped<ICheepService, CheepService>()
    .AddRouting()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie("Cookies")
    .AddGitHub(o =>
    {
        o.ClientId = builder.Environment.IsDevelopment() ? 
            builder.Configuration["development:authentication:github:clientId"] : 
            Environment.GetEnvironmentVariable("clientId", EnvironmentVariableTarget.Machine);
        o.ClientSecret = builder.Environment.IsDevelopment() ? 
            builder.Configuration["development:authentication:github:clientSecret"] : 
            Environment.GetEnvironmentVariable("clientSecret", EnvironmentVariableTarget.Machine);
        o.CallbackPath = "/signin-github";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Auth
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


// Get an instance of ChirpDBContext
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ChirpDBContext>();

// Add template data to the database
DbInitializer.SeedDatabase(context);
context.Database.EnsureCreated();

app.MapRazorPages();


app.Run();
