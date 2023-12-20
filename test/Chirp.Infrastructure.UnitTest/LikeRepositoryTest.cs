using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Infrastructure.UnitTest
{
    public class LikeRepositoryTest
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ChirpDBContext> _contextOptions;

        public LikeRepositoryTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
                .UseSqlite(_connection)
                .Options;

            var context = new ChirpDBContext(_contextOptions);
            context.Database.EnsureCreated();
            context.Authors.AddRange
            (
                new Author { AuthorId = 1, Name = "Jens", Email = "example@mail.com" },
                new Author { AuthorId = 2, Name = "Børge", Email = "example@mail.com" }
            );
            context.SaveChanges();
        }

        [Fact]
        public void LikeCheepAddsLikeToDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };

            // Act
            likeRepository.AddLike(newLike);

            // Assert
            context.Likes.Should().ContainSingle(l => l.AuthorId == newLike.AuthorId && l.CheepId == newLike.CheepId);
        }

        [Fact]
        public void UnlikeCheepRemovesLikeFromDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };
            context.Likes.Add(newLike);
            context.SaveChanges();

            // Act
            likeRepository.RemoveLike(newLike);

            // Assert
            context.Likes.Should().BeEmpty();
        }

        [Fact]
        public void LikeExistsReturnsTrueForExistingLike()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };
            context.Likes.Add(newLike);
            context.SaveChanges();

            // Act
            var result = likeRepository.LikeExists(newLike);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void DeleteAllLikesByAuthorIdRemovesAllLikesByAuthorId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var author = new Author { AuthorId = 1, Email = "example@mail.com", Name = "Example" };
            var newLike1 = new Like { AuthorId = 1, CheepId = 1 };
            var newLike2 = new Like { AuthorId = 1, CheepId = 2 };
            var newLike3 = new Like { AuthorId = 2, CheepId = 1 };
            context.Likes.Add(newLike1);
            context.Likes.Add(newLike2);
            context.Likes.Add(newLike3);
            context.SaveChanges();

            // Act
            likeRepository.DeleteAllLikesByAuthor(author);

            // Assert
            context.Likes.Should().ContainSingle(l => l.AuthorId == 2 && l.CheepId == 1);
        }

        [Fact]
        public void DeleteAllLikesOnCheepsByAuthorIdRemovesAllLikesOnCheepsByAuthorId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var cheepsToStore = new List<Cheep>
            {
                new() { CheepId = 1, AuthorId = 1, Text = "Cheep 1", TimeStamp = DateTime.Now },
                new() { CheepId = 2, AuthorId = 1, Text = "Cheep 2", TimeStamp = DateTime.Now },
                new() { CheepId = 3, AuthorId = 2, Text = "Cheep 3", TimeStamp = DateTime.Now }
            };
            context.Cheeps.AddRange(cheepsToStore);
            context.SaveChanges();
            var author = new Author { AuthorId = 1, Email = "example@mail.com", Name = "Example" };
            var newLike1 = new Like { AuthorId = 1, CheepId = 3 };
            var newLike2 = new Like { AuthorId = 2, CheepId = 2 };
            var newLike3 = new Like { AuthorId = 2, CheepId = 1 };
            context.Likes.Add(newLike1);
            context.Likes.Add(newLike2);
            context.Likes.Add(newLike3);
            context.SaveChanges();

            // Act
            likeRepository.DeleteAllLikesOnCheepsByAuthor(author);

            // Assert
            context.Likes.Should().ContainSingle(l => l.AuthorId == 1 && l.CheepId == 3);
        }
    }
}
