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
        var cheep = new {
            authorId = 1,
            text = "Another test cheep!"
        };

        // Act
        chirpStorage.StoreCheep(cheep.authorId, cheep.text);

        // Assert
        var storedCheep = context.Cheeps.FirstOrDefault();
        Assert.NotNull(storedCheep);
        Assert.Equal(cheep.text, storedCheep.Text);
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
        context.Cheeps.AddRange(cheepsToStore);
        context.SaveChanges();

        // Act
        var cheeps = chirpStorage.QueryCheeps(1, 32, author: "Jens");

        // Assert
        cheeps.Count().Should().Be(2);
    }
}
