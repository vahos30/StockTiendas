using Microsoft.EntityFrameworkCore;
using StockTiendas.Domain.Products;

namespace StockTiendas.Infrastructure.Persistence;

public sealed class TrackedProductRepository(StockTiendasDbContext dbContext) : ITrackedProductRepository
{
    public Task<TrackedProduct?> FindBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Products
            .Include(product => product.Availability)
            .SingleOrDefaultAsync(product => product.Sku == sku, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TrackedProduct>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Include(product => product.Availability)
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TrackedProduct product,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
