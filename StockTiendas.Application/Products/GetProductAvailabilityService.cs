using StockTiendas.Domain.Products;

namespace StockTiendas.Application.Products;

public sealed class GetProductAvailabilityService(IProductAvailabilityProvider provider)
{
    public Task<ProductAvailability> ExecuteAsync(
        GetProductAvailabilityQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.ProductUrl) ||
            !Uri.TryCreate(query.ProductUrl, UriKind.Absolute, out var productUri) ||
            productUri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("La URL del producto no es válida.", nameof(query));
        }

        return provider.GetAvailabilityAsync(productUri, cancellationToken);
    }
}
