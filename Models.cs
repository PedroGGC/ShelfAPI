using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShelfAPI;

// ── Entity ──

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required, Range(0.01, 999999.99)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required, Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// ── DTOs ──

public record CreateProductRequest(
    [Required, StringLength(150, MinimumLength = 2)] string Name,
    [StringLength(500)] string? Description,
    [Required, Range(0.01, 999999.99)] decimal Price,
    [Required, Range(0, int.MaxValue)] int Stock
);

public record UpdateProductRequest(
    [Required, StringLength(150, MinimumLength = 2)] string Name,
    [StringLength(500)] string? Description,
    [Required, Range(0.01, 999999.99)] decimal Price,
    [Required, Range(0, int.MaxValue)] int Stock
);

public record ProductResponse(
    int Id, string Name, string? Description,
    decimal Price, int Stock,
    DateTime CreatedAt, DateTime? UpdatedAt
);

// ── DbContext ──

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasIndex(p => p.Name);
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");
        });
    }
}
