using Chirp.Core;
using Chirp.Infrastructure;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Razor.UnitTest
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
            followRepository.Follow(newFollow.FollowerId, newFollow.FollowedId);

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
            followRepository.Unfollow(newFollow.FollowerId, newFollow.FollowedId);

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
            var result = followRepository.FollowExists(newFollow.FollowerId, newFollow.FollowedId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void FindFollowingByAuthorIdReturnsFollowsForFollowerId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var authorId = 1;
            context.Follows.Add(new Follow { FollowerId = authorId, FollowedId = 2 });
            context.Follows.Add(new Follow { FollowerId = authorId, FollowedId = 3 });
            context.SaveChanges();

            // Act
            var result = followRepository.FindFollowingByAuthorId(authorId);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void FindFollowersCountByAuthorIdReturnsNumberOfFollowsForFollowedId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var followRepository = new FollowRepository(context);

            // Arrange
            var authorId = 2;
            context.Follows.Add(new Follow { FollowerId = 1, FollowedId = authorId });
            context.Follows.Add(new Follow { FollowerId = 3, FollowedId = authorId });
            context.Follows.Add(new Follow { FollowerId = 4, FollowedId = authorId });
            context.SaveChanges();

            // Act
            var result = followRepository.FindFollowersCountByAuthorId(authorId);

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
            Assert.Throws<DbUpdateException>(() => followRepository.Follow(newFollow.FollowerId, newFollow.FollowedId));
        }
    }
}
