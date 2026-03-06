using Microsoft.EntityFrameworkCore;
using ShelfAPI.Models;

namespace ShelfAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasIndex(p => p.Name);
            entity.Property(p => p.Price).HasColumnType("decimal(10,2)");
        });
    }
}
