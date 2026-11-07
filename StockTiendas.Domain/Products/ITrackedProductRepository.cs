namespace StockTiendas.Domain.Products;

public interface ITrackedProductRepository
{
    Task<TrackedProduct?> FindBySkuAsync(string sku, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TrackedProduct>> ListAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TrackedProduct product, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
