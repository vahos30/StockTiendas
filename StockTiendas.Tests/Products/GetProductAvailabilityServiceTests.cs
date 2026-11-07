using StockTiendas.Application.Products;
using StockTiendas.Domain.Products;
using StockTiendas.Infrastructure.PriceSmart;
using Microsoft.Extensions.Options;

namespace StockTiendas.Tests.Products;

public sealed class GetProductAvailabilityServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidUrl_DelegatesToProvider()
    {
        var expected = new ProductAvailability(
            "502626-40094581498",
            "Hamilton Beach",
            true,
            [],
            DateTimeOffset.UtcNow);
        var provider = new FakeProvider(expected);
        var service = new GetProductAvailabilityService(provider);

        var result = await service.ExecuteAsync(new GetProductAvailabilityQuery(
            "https://www.pricesmart.com/es-co/producto/502626-40094581498"));

        Assert.Same(expected, result);
        Assert.Equal("https://www.pricesmart.com/es-co/producto/502626-40094581498", provider.ReceivedUri?.ToString());
    }

    [Fact]
    public async Task PriceSmartProvider_ReturnsOnlyTargetClubs()
    {
        const string responseJson = """
        {
          "data": {
            "products": {
              "results": [{
                "masterData": {"current": {
                  "name": "Hamilton Beach",
                  "masterVariant": {
                    "sku": "502626-40094581498",
                    "availability": {"channels": {"results": [
                      {"channel": {"key": "6106", "nameAllLocales": [{"locale": "es-CO", "value": "Las Américas"}], "address": {"country": "CO"}}, "availability": {"isOnStock": true, "availableQuantity": 60}},
                      {"channel": {"key": "6110", "nameAllLocales": [{"locale": "es-CO", "value": "El Poblado"}], "address": {"country": "CO"}}, "availability": {"isOnStock": true, "availableQuantity": 56}},
                      {"channel": {"key": "6107", "nameAllLocales": [{"locale": "es-CO", "value": "Otro club"}], "address": {"country": "CO"}}, "availability": {"isOnStock": true, "availableQuantity": 10}}
                    ]}}
                  }
                }}
              }]
            }
          }
        }
        """;
        using var handler = new StubHttpHandler(responseJson);
        using var client = new HttpClient(handler);
        var provider = new PriceSmartAvailabilityProvider(
            client,
            Options.Create(new PriceSmartOptions()));

        var result = await provider.GetAvailabilityAsync(
            new Uri("https://www.pricesmart.com/es-co/producto/502626-40094581498"));

        Assert.Equal(["6106", "6110"], result.Stores.Select(store => store.Code));
        Assert.Equal([60, 56], result.Stores.Select(store => store.AvailableQuantity));
        Assert.Equal("Las Américas", result.Stores.First().Name);
        Assert.Equal(HttpMethod.Post, handler.Method);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/product")]
    public async Task ExecuteAsync_WithInvalidUrl_ThrowsArgumentException(string url)
    {
        var service = new GetProductAvailabilityService(new FakeProvider());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ExecuteAsync(new GetProductAvailabilityQuery(url)));
    }

    private sealed class FakeProvider(ProductAvailability? result = null) : IProductAvailabilityProvider
    {
        public Uri? ReceivedUri { get; private set; }

        public Task<ProductAvailability> GetAvailabilityAsync(
            Uri productUri,
            CancellationToken cancellationToken = default)
        {
            ReceivedUri = productUri;
            return Task.FromResult(result ?? new ProductAvailability(
                "product",
                "Product",
                false,
                [],
                DateTimeOffset.UtcNow));
        }
    }

    private sealed class StubHttpHandler(string responseJson) : HttpMessageHandler
    {
        public HttpMethod? Method { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }
}
