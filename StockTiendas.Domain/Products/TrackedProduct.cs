namespace StockTiendas.Domain.Products;

public sealed class TrackedProduct
{
    public int Id { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastCheckedAt { get; set; }
    public List<TrackedProductClubAvailability> Availability { get; set; } = [];
}
