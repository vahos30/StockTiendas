namespace StockTiendas.Domain.Products;

public interface IProductAvailabilityProvider
{
    Task<ProductAvailability> GetAvailabilityAsync(
        Uri productUri,
        CancellationToken cancellationToken = default);
}
