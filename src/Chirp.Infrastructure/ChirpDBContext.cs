using Chirp.Core;
using Chirp.Core.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the database context for the Chirp application, handling the interaction
/// with the underlying database for Cheeps, Authors, Likes, and Follows.
/// </summary>
public class ChirpDBContext : DbContext
{
    /// <summary>
    /// The DbSet for Cheeps, representing chirp messages in the application.
    /// </summary>
    public DbSet<Cheep> Cheeps { get; set; }

    /// <summary>
    /// The DbSet for Authors, representing users who create chirps in the application.
    /// </summary>
    public DbSet<Author> Authors { get; set; }

    /// <summary>
    /// The DbSet for Likes, representing the likes given by authors to cheeps.
    /// </summary>
    public DbSet<Like> Likes { get; set; }

    /// <summary>
    /// The DbSet for Follows, representing the follow relationships between authors.
    /// </summary>
    public DbSet<Follow> Follows { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChirpDBContext"/> class with the specified options.
    /// </summary>
    /// <param name="options"> The options for configuring the database context </param>
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model for the Chirp database, including entity relationships and constraints.
    /// </summary>
    /// <param name="modelBuilder"> The builder used to construct the model for the database context. </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cheep>().Property(x => x.CheepId).ValueGeneratedNever();
        modelBuilder.Entity<Author>().Property(x => x.AuthorId).ValueGeneratedNever();

        // Prevents an Author from liking a cheep more than once
        modelBuilder.Entity<Like>().HasKey(x => new { x.AuthorId, x.CheepId });

        // Prevents an Author from following another Author more than once
        modelBuilder.Entity<Follow>().HasKey(x => new { x.FollowerId, x.FollowedId });

        // Prevents an Author from following themselves
        modelBuilder.Entity<Follow>()
            .ToTable(t => t.HasCheckConstraint("CK_FollowerNotEqualFollowed", "FollowerId <> FollowedId"));
    }
}
