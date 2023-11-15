using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;
public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Follow> Follows { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
    }

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
