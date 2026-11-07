using Microsoft.EntityFrameworkCore;
using StockTiendas.Domain.Products;

namespace StockTiendas.Infrastructure.Persistence;

public sealed class StockTiendasDbContext(DbContextOptions<StockTiendasDbContext> options)
    : DbContext(options)
{
    public DbSet<TrackedProduct> Products => Set<TrackedProduct>();
    public DbSet<TrackedProductClubAvailability> ClubAvailability => Set<TrackedProductClubAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedProduct>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Sku).HasMaxLength(100).IsRequired();
            entity.Property(product => product.Name).HasMaxLength(300).IsRequired();
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasMany(product => product.Availability)
                .WithOne(availability => availability.Product)
                .HasForeignKey(availability => availability.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TrackedProductClubAvailability>(entity =>
        {
            entity.ToTable("ProductClubAvailability");
            entity.HasKey(availability => availability.Id);
            entity.Property(availability => availability.ClubCode).HasMaxLength(20).IsRequired();
            entity.Property(availability => availability.ClubName).HasMaxLength(200).IsRequired();
            entity.HasIndex(availability => new { availability.ProductId, availability.ClubCode })
                .IsUnique();
        });
    }
}
