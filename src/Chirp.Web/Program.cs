
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

SecretClientOptions options = new SecretClientOptions()
{
    Retry =
    {
        Delay= TimeSpan.FromSeconds(2),
        MaxDelay = TimeSpan.FromSeconds(16),
        MaxRetries = 5,
        Mode = RetryMode.Exponential
    }
};
var client = new SecretClient(new Uri("https://ChirpKeyVault.vault.azure.net/"), new DefaultAzureCredential(),options);
//
// KeyVaultSecret secretId = client.GetSecret("clientId");
// KeyVaultSecret secret = client.GetSecret("clientSecret");
//
// string secretIdValue = secretId.Value;
// string secretValue = secret.Value;
// Console.WriteLine(secretIdValue);
// Console.WriteLine(secretValue);
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
            client.GetSecret("clientId").Value.Value;
        o.ClientSecret = builder.Environment.IsDevelopment() ? 
            builder.Configuration["development:authentication:github:clientSecret"] : 
            client.GetSecret("clientSecret").Value.Value;
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
