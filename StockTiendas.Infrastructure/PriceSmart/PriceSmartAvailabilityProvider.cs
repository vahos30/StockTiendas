using System.Net.Http.Json;
using System.Text.Json;
using StockTiendas.Domain.Products;

namespace StockTiendas.Infrastructure.PriceSmart;

public sealed class PriceSmartAvailabilityProvider(
    HttpClient httpClient,
    Microsoft.Extensions.Options.IOptions<PriceSmartOptions> options) : IProductAvailabilityProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly PriceSmartOptions settings = options.Value;

    public async Task<ProductAvailability> GetAvailabilityAsync(
        Uri productUri,
        CancellationToken cancellationToken = default)
    {
        var productId = ExtractProductId(productUri);
        var request = new object[]
        {
            new { skus = new[] { productId } },
            new
            {
                products = "getProductBySKU",
                metadata = new { channelId = settings.ColombiaChannelId }
            }
        };

        using var response = await httpClient.PostAsJsonAsync(
            settings.ProductEndpoint,
            request,
            SerializerOptions,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PriceSmartProductResponse>(
            SerializerOptions,
            cancellationToken);
        var product = payload?.Data?.Products?.Results.FirstOrDefault();
        var current = product?.MasterData?.Current
            ?? throw new InvalidOperationException("PriceSmart no devolvió información del producto.");

        var targetClubs = current.MasterVariant?.Availability?.Channels?.Results
            .Where(IsTargetClub)
            .Select(MapStore)
            .ToArray() ?? [];

        return new ProductAvailability(
            ProductId: current.MasterVariant?.Sku ?? productId,
            Name: current.Name ?? productId,
            IsAvailable: targetClubs.Any(store => store.IsAvailable),
            Stores: targetClubs,
            CheckedAt: DateTimeOffset.UtcNow);
    }

    private bool IsTargetClub(ProductChannelResult result)
    {
        var key = result.Channel?.Key;
        return key is not null &&
               settings.TargetClubCodes.Contains(key, StringComparer.Ordinal) &&
               string.Equals(result.Channel?.Address?.Country, "CO", StringComparison.OrdinalIgnoreCase);
    }

    private static StoreAvailability MapStore(ProductChannelResult result)
    {
        var channel = result.Channel!;
        var name = channel.Names.FirstOrDefault(name =>
            string.Equals(name.Locale, "es", StringComparison.OrdinalIgnoreCase))?.Value
            ?? channel.Names.FirstOrDefault()?.Value
            ?? channel.Key
            ?? "Club desconocido";
        var availability = result.Availability;

        return new StoreAvailability(
            Code: channel.Key ?? string.Empty,
            Name: name,
            IsAvailable: availability?.IsOnStock ?? false,
            AvailableQuantity: availability?.AvailableQuantity ?? 0);
    }

    private static string ExtractProductId(Uri productUri)
    {
        var productId = productUri.Segments
            .Select(segment => segment.Trim('/'))
            .LastOrDefault(segment => !string.IsNullOrWhiteSpace(segment));

        return productId
            ?? throw new ArgumentException("La URL no contiene un SKU válido.", nameof(productUri));
    }
}
