
using Chirp.Razor;
using Chirp.Razor.Shared.Storage;
using Chirp.Razor.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var dbPath = StoragePathHandler.getPathToLocalFolder();

builder.Services
    .AddDbContext<ChirpDBContext>(options => options.UseSqlite($"Data Source={dbPath}"))
    .AddScoped<IChirpStorage, ChirpStorage>()
    .AddScoped<ICheepService, CheepService>()
;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Get an instance of ChirpDBContext
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ChirpDBContext>();

// Add template data to the database
DbInitializer.SeedDatabase(context);
context.Database.EnsureCreated();

app.MapRazorPages();

app.Run();
