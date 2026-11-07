using StockTiendas.Domain.Products;

namespace StockTiendas.Application.Products;

public sealed class RegisterProductService(
    ITrackedProductRepository repository,
    IProductAvailabilityProvider availabilityProvider)
{
    public async Task<TrackedProduct> ExecuteAsync(
        RegisterProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(command));
        }

        var sku = command.Sku?.Trim();

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("El SKU del producto es obligatorio.", nameof(command));
        }

        if (sku.Any(char.IsControl))
        {
            throw new ArgumentException("El SKU del producto contiene caracteres no válidos.", nameof(command));
        }

        var productUrl = new Uri($"https://www.pricesmart.com/es-co/producto/{Uri.EscapeDataString(sku)}");
        var availability = await availabilityProvider.GetAvailabilityAsync(productUrl, cancellationToken);
        var product = await repository.FindBySkuAsync(sku, cancellationToken);

        if (product is null)
        {
            product = new TrackedProduct
            {
                Sku = sku,
                Name = command.Name.Trim(),
                CreatedAt = DateTimeOffset.UtcNow
            };
            await repository.AddAsync(product, cancellationToken);
        }
        else
        {
            product.Name = command.Name.Trim();
            product.Availability.Clear();
        }

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
        await repository.SaveChangesAsync(cancellationToken);
        return product;
    }
}
