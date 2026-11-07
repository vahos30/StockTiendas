namespace StockTiendas.Domain.Products;

public sealed record ProductAvailability(
    string ProductId,
    string Name,
    bool IsAvailable,
    IReadOnlyCollection<StoreAvailability> Stores,
    DateTimeOffset CheckedAt);
