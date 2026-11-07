namespace StockTiendas.Infrastructure.PriceSmart;

public sealed class PriceSmartOptions
{
    public Uri ProductEndpoint { get; set; } = new("https://www.pricesmart.com/api/ct/getProduct");

    public string ColombiaChannelId { get; set; } = "f1f5bd48-9b07-490b-a2b5-1132a0678fd5";

    public string[] TargetClubCodes { get; set; } = ["6106", "6110"];
}
