using System.Configuration;
using System.Security.Claims;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Chirp.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
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

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile(builder.Environment.IsDevelopment() ? "appsettings.Development.json" :"appsettings.json")
    .Build();

string clientId;
string clientSecret;
string connectionString;
// Creates a secret client which connects to our azure key vault
if (!builder.Environment.IsDevelopment())
{
    var client = new SecretClient(new Uri("https://ChirpKeyVault.vault.azure.net/"), new DefaultAzureCredential(),options);
    clientId = client.GetSecret("clientId").Value.Value;
    clientSecret = client.GetSecret("clientSecret").Value.Value;
    connectionString = client.GetSecret("bdsagroup3-chirpdb").Value.Value;
}
else
{
    clientId = builder.Configuration["development:authentication:github:clientId"] ?? throw new ConfigurationErrorsException("Expected the secret development:authentication:github:clientId to be set but found null");
    clientSecret = builder.Configuration["development:authentication:github:clientSecret"] ?? throw new ConfigurationErrorsException("Expected the secret development:authentication:github:clientSecret to be set but found null");
    connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ConfigurationErrorsException("Connection string could not be found");
}

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services
    .AddDbContext<ChirpDBContext>(options => options.UseSqlServer(connectionString))
    .AddScoped<ICheepRepository, CheepRepository>()
    .AddScoped<IAuthorRepository, AuthorRepository>()
    .AddScoped<ILikeRepository, LikeRepository>()
    .AddScoped<IFollowRepository, FollowRepository>()
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
        o.ClientId = clientId;
        o.ClientSecret = clientSecret;
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email");
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
