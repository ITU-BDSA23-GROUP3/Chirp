using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Infrastructure.UnitTest
{
    public class FollowRepositoryTest
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ChirpDBContext> _contextOptions;

        public FollowRepositoryTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
                .UseSqlite(_connection)
                .Options;

            var context = new ChirpDBContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        [Fact]
        public void FollowAddsFollowToDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var newFollow = new Follow { FollowerId = 1, FollowedId = 2 };

            // Act
            followRepository.AddFollow(newFollow);

            // Assert
            context.Follows.Should().ContainSingle(f => f.FollowerId == newFollow.FollowerId && f.FollowedId == newFollow.FollowedId);
        }

        [Fact]
        public void UnfollowRemovesFollowFromDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var newFollow = new Follow { FollowerId = 1, FollowedId = 2 };
            context.Follows.Add(newFollow);
            context.SaveChanges();

            // Act
            followRepository.RemoveFollow(newFollow);

            // Assert
            context.Follows.Should().BeEmpty();
        }

        [Fact]
        public void FollowExistsReturnsTrueForExistingFollow()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var newFollow = new Follow { FollowerId = 1, FollowedId = 2 };
            context.Follows.Add(newFollow);
            context.SaveChanges();

            // Act
            var result = followRepository.FollowExists(newFollow);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void FindFollowingByAuthorIdReturnsFollowsForFollowerId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var author = new Author { AuthorId= 1, Email="example@mail.com", Name = "Example"};
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = 2 });
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = 3 });
            context.SaveChanges();

            // Act
            var result = followRepository.FindFollowingByAuthor(author);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void FindFollowersCountByAuthorIdReturnsNumberOfFollowsForFollowedId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var author = new Author { AuthorId= 2, Email="example@mail.com", Name = "Example"};
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = 2 });
            context.Follows.Add(new Follow { FollowerId = 3, FollowedId = 2 });
            context.Follows.Add(new Follow { FollowerId = 4, FollowedId = 2 });
            context.SaveChanges();

            // Act
            var result = followRepository.FindFollowersCountByAuthor(author);

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public void FollowThrowsExceptionForIdenticalIds()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var newFollow = new Follow { FollowerId = 1, FollowedId = 1 };

            // Act and Assert
            Assert.Throws<DbUpdateException>(() => followRepository.AddFollow(newFollow));
        }

        [Fact]
        public void DeleteAllFollowsByAuthorIdRemovesAllFollowsForAuthorId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var author = new Author { AuthorId= 1, Email="example@mail.com", Name = "Example"};
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = 2 });
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = 3 });
            context.Follows.Add(new Follow { FollowerId = 2, FollowedId = 1 });
            context.Follows.Add(new Follow { FollowerId = 2, FollowedId = 3 });
            context.SaveChanges();

            // Act
            followRepository.DeleteAllFollowsByAuthor(author);

            // Assert
            context.Follows.Should().ContainSingle(f => f.FollowerId == 2 && f.FollowedId == 3);
        }
    }
}
