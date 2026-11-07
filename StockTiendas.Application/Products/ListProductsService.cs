using StockTiendas.Domain.Products;

namespace StockTiendas.Application.Products;

public sealed class ListProductsService(ITrackedProductRepository repository)
{
    public Task<IReadOnlyCollection<TrackedProduct>> ExecuteAsync(
        ListProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        return repository.ListAsync(cancellationToken);
    }
}
