using Chirp.Core;
using Microsoft.EntityFrameworkCore;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    { 
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cheep>().Property(e => e.CheepId).ValueGeneratedNever();
        modelBuilder.Entity<Author>().Property(x => x.AuthorId).ValueGeneratedNever();
    }
}
