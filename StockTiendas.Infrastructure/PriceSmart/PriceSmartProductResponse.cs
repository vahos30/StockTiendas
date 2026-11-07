using System.Text.Json.Serialization;

namespace StockTiendas.Infrastructure.PriceSmart;

internal sealed class PriceSmartProductResponse
{
    [JsonPropertyName("data")]
    public ProductData? Data { get; set; }
}

internal sealed class ProductData
{
    [JsonPropertyName("products")]
    public ProductQueryResult? Products { get; set; }
}

internal sealed class ProductQueryResult
{
    [JsonPropertyName("results")]
    public List<ProductResult> Results { get; set; } = [];
}

internal sealed class ProductResult
{
    [JsonPropertyName("masterData")]
    public MasterData? MasterData { get; set; }
}

internal sealed class MasterData
{
    [JsonPropertyName("current")]
    public ProductDataCurrent? Current { get; set; }
}

internal sealed class ProductDataCurrent
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("masterVariant")]
    public ProductVariant? MasterVariant { get; set; }
}

internal sealed class ProductVariant
{
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("availability")]
    public ProductAvailabilityData? Availability { get; set; }
}

internal sealed class ProductAvailabilityData
{
    [JsonPropertyName("channels")]
    public ProductChannelResults? Channels { get; set; }
}

internal sealed class ProductChannelResults
{
    [JsonPropertyName("results")]
    public List<ProductChannelResult> Results { get; set; } = [];
}

internal sealed class ProductChannelResult
{
    [JsonPropertyName("availability")]
    public ChannelAvailability? Availability { get; set; }

    [JsonPropertyName("channel")]
    public Channel? Channel { get; set; }
}

internal sealed class ChannelAvailability
{
    [JsonPropertyName("isOnStock")]
    public bool IsOnStock { get; set; }

    [JsonPropertyName("availableQuantity")]
    public int AvailableQuantity { get; set; }
}

internal sealed class Channel
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("nameAllLocales")]
    public List<LocalizedName> Names { get; set; } = [];

    [JsonPropertyName("address")]
    public ChannelAddress? Address { get; set; }
}

internal sealed class LocalizedName
{
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

internal sealed class ChannelAddress
{
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
