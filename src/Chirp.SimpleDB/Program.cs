using Chirp.SimpleDB.Providers;
using Chirp.SimpleDB.Storage;
using Chirp.SimpleDB.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IStorageProvider<StorageChirpRecord>, ChirpStorageProvider>();
builder.Services.AddControllers().AddNewtonsoftJson();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Not useful right now
// app.UseAuthorization();

app.MapControllers();

app.Run();
