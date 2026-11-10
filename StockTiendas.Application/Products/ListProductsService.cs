using StockTiendas.Domain.Products;

namespace StockTiendas.Application.Products;

public sealed class ListProductsService(
    ITrackedProductRepository repository,
    IProductAvailabilityProvider availabilityProvider)
{
    public async Task<IReadOnlyCollection<TrackedProduct>> ExecuteAsync(
        ListProductsQuery query,
        bool refresh = false,
        CancellationToken cancellationToken = default)
    {
        var products = await repository.ListAsync(cancellationToken);

        if (!refresh)
        {
            return products;
        }

        foreach (var product in products)
        {
            var productUrl = new Uri($"https://www.pricesmart.com/es-co/producto/{product.Sku}");
            var availability = await availabilityProvider.GetAvailabilityAsync(productUrl, cancellationToken);

            product.Availability.Clear();
            foreach (var club in availability.Stores)
            {
                product.Availability.Add(new TrackedProductClubAvailability
                {
                    ClubCode = club.Code,
                    ClubName = club.Name,
                    IsAvailable = club.IsAvailable,
                    AvailableQuantity = club.AvailableQuantity,
                    CheckedAt = availability.CheckedAt
                });
            }

            product.LastCheckedAt = availability.CheckedAt;
        }

        await repository.SaveChangesAsync(cancellationToken);
        return products;
    }
}

