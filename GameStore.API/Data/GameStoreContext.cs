using System;
using Microsoft.EntityFrameworkCore;
using GameStore.API.Models;

namespace GameStore.API.Data;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> options) : base(options) { }

    // DbSet for Games
    public DbSet<Game> Games => Set<Game>();

    // DbSet for Genres
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            entity.Property(g => g.Genre).HasMaxLength(50);
            entity.Property(g => g.Price).HasColumnType("decimal(18,2)"); // Explicitly specify column type
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
        });
    }
}
