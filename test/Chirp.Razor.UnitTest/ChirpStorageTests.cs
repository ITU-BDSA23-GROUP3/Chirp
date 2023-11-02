using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Data.Sqlite;
using FluentAssertions;
using Chirp.Infrastructure.Storage;

namespace Chirp.Razor.UnitTest;
public class ChirpStorageTests
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ChirpDBContext> _contextOptions;

    public ChirpStorageTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureCreated();

        context.Authors.AddRange(
            new Author { AuthorId = 1, Name = "Jens", Email = "test@mail.dk" },
            new Author { AuthorId = 2, Name = "Børge", Email = "wow@dd.dk" }
        );
        context.SaveChanges();

    }

    [Fact]
    public void QueryCheepCountReturnsCorrectCount()
    {
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        context.AddRange(
            new Cheep { AuthorId = 1, Text = "Wow it works!", TimeStamp = DateTime.Now }
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
        var context = new ChirpDBContext(_contextOptions);
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
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheeps = new List<Cheep>{
            new Cheep {
                AuthorId = 1, Text = "How u doin'?", TimeStamp = DateTime.Now
            },
            new Cheep {
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
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheepsToStore = new List<Cheep>
        {
            new() { AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new() { AuthorId = 1, Text = "Cheep 2", TimeStamp = DateTime.Now },
            new() { AuthorId = 2, Text = "Cheep 3", TimeStamp = DateTime.Now }
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
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new ChirpRepository(context);

        // Arrange
        var cheep = new Cheep
        {
            Text = "A Cheep without AuthorId",
            TimeStamp = DateTime.Now
        };

        // Act and Assert
        Assert.Throws<DbUpdateException>(() => chirpStorage.StoreCheep(cheep));
    }

    [Fact]
    public void CreateAuthorStoresNewAuthorInDb()
    {
        var context = new ChirpDBContext(_contextOptions);
        var authorRepository = new AuthorRepository(context);

        // Arrange
        var authorsBefore = context.Authors.Count();

        var newAuthor = new Author
        {
            Name = "Name and",
            Email = "Email pair that isn't in the db",
        };

        // Act
        authorRepository.CreateAuthor(newAuthor.Name, newAuthor.Email);

        // Assert
        context.Authors.Should().HaveCount(authorsBefore+1);
    }

    [Fact]
    public void FindAuthorByNameReturnsAListOfAuthors()
    {
        var context = new ChirpDBContext(_contextOptions);
        var authorRepository = new AuthorRepository(context);

        // Arrange
        var authorsWithTheSameName = new List<Author>
        {
            new () { Name = "Ida", Email = "ida1@" },
            new () { Name = "Ida", Email = "ida2@" }
        };
        context.Authors.AddRange(authorsWithTheSameName);
        context.SaveChanges();

        // Act and Assert
        authorRepository.FindAuthorsByName("Ida").Should().HaveCount(2);
    }

    [Fact]
    public void CreatedAuthorGetsAutogeneratedId()
    {
        var context = new ChirpDBContext(_contextOptions);
        var authorRepository = new AuthorRepository(context);

        // Arrange
        var authorsBefore = context.Authors.Count();

        var newAuthor = new Author
        {
            Name = "Hans Hansen", Email = "UniqueMail898989",
        };

        // Act
        authorRepository.CreateAuthor(newAuthor.Name, newAuthor.Email);

        // Assert
        authorRepository.FindAuthorsByEmail(newAuthor.Email).First().AuthorId.Should().BeInRange(0,authorsBefore+1);
    }
}
