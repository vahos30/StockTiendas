using System.Text.Json.Serialization;

namespace StockTiendas.Domain.Products;

public sealed class TrackedProductClubAvailability
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public required string ClubCode { get; set; }
    public required string ClubName { get; set; }
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTimeOffset CheckedAt { get; set; }
    [JsonIgnore]
    public TrackedProduct? Product { get; set; }
}
