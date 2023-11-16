﻿using Chirp.Core;
using Chirp.Infrastructure;
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
            new Author { AuthorId = 1, Name = "Jens", Email = "test@mail.dk" },
            new Author { AuthorId = 2, Name = "Børge", Email = "wow@dd.dk" }
        );
        context.SaveChanges();

        followRepository = new FollowRepository(context);
        authorRepository = new AuthorRepository(context);
    }

    [Fact]
    public void QueryCheepCountReturnsCorrectCount()
    {
        var context = new ChirpDBContext(_contextOptions);
        var chirpStorage = new CheepRepository(context, followRepository, authorRepository);

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
        var chirpStorage = new CheepRepository(context, followRepository, authorRepository);

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
        var chirpStorage = new CheepRepository(context, followRepository, authorRepository);

        // Arrange
        var cheeps = new List<Cheep>{
            new Cheep {
                CheepId = 1, AuthorId = 1, Text = "How u doin'?", TimeStamp = DateTime.Now
            },
            new Cheep {
                CheepId = 2, AuthorId = 2, Text = "Hey all", TimeStamp = DateTime.Now
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
        var chirpStorage = new CheepRepository(context, followRepository, authorRepository);

        // Arrange
        var cheepsToStore = new List<Cheep>
        {
            new() { CheepId = 1, AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
            new() { CheepId = 2, AuthorId = 1, Text = "Cheep 2", TimeStamp = DateTime.Now },
            new() { CheepId = 3, AuthorId = 2, Text = "Cheep 3", TimeStamp = DateTime.Now }
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
        var chirpStorage = new CheepRepository(context, followRepository, authorRepository);

        // Arrange
        var cheep = new Cheep
        {
            Text = "A Cheep without AuthorId",
            TimeStamp = DateTime.Now
        };

        // Act and Assert
        Assert.Throws<DbUpdateException>(() => chirpStorage.StoreCheep(cheep));
    }
}