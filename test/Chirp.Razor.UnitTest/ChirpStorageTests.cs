using Chirp.Core;
using Chirp.Infrastructure;
using ChirpDBContext = Chirp.Infrastructure.ChirpDBContext;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace Chirp.Razor.UnitTest;
public class ChirpStorageTests : IDisposable
{
    private readonly DbContextOptions<ChirpDBContext> _contextOptions;

    public ChirpStorageTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(databaseName: "ChirpDB")
            .Options;

        using var context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureDeleted();

        context.Database.EnsureCreated();

        context.Authors.AddRange(
            new Author { Name = "Jens", Email = "test@mail.dk" },
            new Author { Name = "Børge", Email = "wow@dd.dk" }
        );
        context.SaveChanges();
    }

    public void Dispose()
    {
        using var context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureDeleted();
    }

    [Fact]
    public void QueryCheepCountReturnsCorrectCount()
    {
        using var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        context.AddRange(
            new Cheep {AuthorId=1, Text = "Wow it works!", TimeStamp = DateTime.Now }
        );
        context.SaveChanges();

        // Act
        var count = chirpStorage.QueryCheepCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void StoreCheepSavesCheep()
    {
        using var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheep = new Cheep
        {
            AuthorId = 1,
            Text = "Another test cheep!",
            TimeStamp = DateTime.Now
        };

        // Act
        chirpStorage.StoreCheep(cheep);

        // Assert
        var storedCheep = context.Cheeps.FirstOrDefault();
        Assert.NotNull(storedCheep);
        Assert.Equal(cheep.Text, storedCheep.Text);
    }

    [Fact]
    public void StoreCheepsSavesCheeps()
    {
        using var context = new ChirpDBContext(_contextOptions);

        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheeps = new List<Cheep>{
            new() {
                AuthorId = 1, Text = "How u doin'?", TimeStamp = DateTime.Now
            },
            new() {
               AuthorId = 2, Text = "Hey all", TimeStamp = DateTime.Now
            }
        };

        // Act
        chirpStorage.StoreCheeps(cheeps);

        // Assert
        context.Cheeps.Should().BeEquivalentTo(cheeps);
    }

    [Fact]
    public void QueryCheepsReturnsCheepsByAuthor()
    {
        using var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheepsToStore = new List<Cheep>
        {
            new() { AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new() { AuthorId = 2, Text = "Cheep 2", TimeStamp = DateTime.Now },
            new() { AuthorId = 3, Text = "Cheep 3", TimeStamp = DateTime.Now }
        };
        context.AddRange(cheepsToStore);
        context.SaveChanges();

        // Act
        var cheeps = chirpStorage.QueryCheeps(0, 32, author: "Jens");

        // Assert
        cheeps.Should().BeEquivalentTo(cheepsToStore.Where(cheep => cheep.AuthorId == 1));
    }

    [Fact]
    public void CheepViolatesFKConstraint()
    {
        using var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheep = new Cheep
        {
            Text = "A Cheep without AuthorId",
            TimeStamp = DateTime.Now
        };

        // Act and Assert
        Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() => chirpStorage.StoreCheep(cheep));
    }
}
