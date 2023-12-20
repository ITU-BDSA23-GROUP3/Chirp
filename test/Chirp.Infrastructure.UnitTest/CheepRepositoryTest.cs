using Chirp.Core.Entities;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Data.Sqlite;
using FluentAssertions;

namespace Chirp.Infrastructure.UnitTest;
public class CheepRepositoryTest
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ChirpDBContext> _contextOptions;
    private IFollowRepository followRepository;
    private IAuthorRepository authorRepository;

    public CheepRepositoryTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureCreated();

        context.Authors.AddRange(
            new Author { AuthorId = 1, Name = "Jens", Email = "example@mail.com" },
            new Author { AuthorId = 2, Name = "Børge", Email = "example@mail.com" }
        );
        context.SaveChanges();

        followRepository = new FollowRepository(context);
        authorRepository = new AuthorRepository(context);
    }

    [Fact]
    public void QueryCheepCountReturnsCorrectCount()
    {
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new CheepRepository(context);

        // Arrange
        context.AddRange(
            new Cheep { AuthorId = 1, Text = "Wow it works!", TimeStamp = DateTime.Now }
        );
        context.SaveChanges();

        // Act
        var count = chirpStorage.GetQueryableCheeps(new List<int>()).Count();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void StoreCheepSavesCheep()
    {
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new CheepRepository(context);

        // Arrange
        var cheep = new Cheep {
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
        var chirpStorage = new CheepRepository(context);

        // Arrange
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 1, AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, AuthorId = 2, Text = "Cheep 2", TimeStamp = DateTime.Now}
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
        var chirpStorage = new CheepRepository(context);

        // Arrange
        var author1 = new Author{ AuthorId=1, Email="example@mail.com", Name="Jens"};

        var cheepsToStore = new List<Cheep>
        {
            new() { CheepId = 1, AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new() { CheepId = 2, AuthorId = 1, Text = "Cheep 2", TimeStamp = DateTime.Now },
            new() { CheepId = 3, AuthorId = 2, Text = "Cheep 3", TimeStamp = DateTime.Now }
        };
        context.Cheeps.AddRange(cheepsToStore);
        context.SaveChanges();

        // Act
        var cheeps = chirpStorage.GetQueryableCheeps(new List<int>(), author1);

        // Assert
        cheeps.Count().Should().Be(2);
    }

    [Fact]
    public void DeleteCheepRemovesCheepFromDatabase()
    {
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new CheepRepository(context);

        // Arrange
        var cheep = new Cheep { AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now };
        context.Cheeps.Add(cheep);
        context.SaveChanges();

        // Act
        chirpStorage.DeleteCheep(cheep);

        // Assert
        context.Cheeps.Should().BeEmpty();
    }

    [Fact]
    public void DeleteAllCheepsByAuthorIdDeletesCheepsFromDatabase()
    {
        var context = new ChirpDBContext(_contextOptions);
        var cheepRepository = new CheepRepository(context);

        var author = new Author { AuthorId = 1, Email = "example@mail.com", Name = "Eksempel" };

        // Arrange
        var cheeps = new List<Cheep>
        {
            new() { CheepId = 1, AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new() { CheepId = 2, AuthorId = 1, Text = "Cheep 2", TimeStamp = DateTime.Now },
            new() { CheepId = 3, AuthorId = 1, Text = "Cheep 3", TimeStamp = DateTime.Now }
        };
        var dontDeleteCheep = new Cheep { CheepId = 4, AuthorId = 2, Text = "Cheep 4", TimeStamp = DateTime.Now };

        context.Cheeps.AddRange(cheeps);
        context.Cheeps.Add(dontDeleteCheep);
        context.SaveChanges();

        // Act
        cheepRepository.DeleteAllCheepsByAuthor(author);

        // Assert
        context.Cheeps.Should().ContainSingle(c => c.AuthorId == dontDeleteCheep.AuthorId);
    }
}
