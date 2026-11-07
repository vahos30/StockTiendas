namespace StockTiendas.Domain.Products;

public sealed record StoreAvailability(
    string Code,
    string Name,
    bool IsAvailable,
    int AvailableQuantity);
