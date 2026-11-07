using StockTiendas.Application.Products;
using StockTiendas.Domain.Products;

namespace StockTiendas.Tests.Products;

public sealed class RegisterProductServiceTests
{
    [Fact]
    public async Task ExecuteAsync_StoresProductAndClubAvailability()
    {
        var repository = new FakeRepository();
        var provider = new FakeAvailabilityProvider();
        var service = new RegisterProductService(repository, provider);

        var result = await service.ExecuteAsync(
            new RegisterProductCommand("Producto de prueba", "123-456"));

        Assert.Equal("Producto de prueba", result.Name);
        Assert.Equal("123-456", result.Sku);
        Assert.Equal(2, result.Availability.Count);
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    private sealed class FakeAvailabilityProvider : IProductAvailabilityProvider
    {
        public Task<ProductAvailability> GetAvailabilityAsync(
            Uri productUri,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ProductAvailability(
                "123-456",
                "Producto de PriceSmart",
                true,
                [
                    new StoreAvailability("6106", "Las Américas", true, 10),
                    new StoreAvailability("6110", "El Poblado", false, 0)
                ],
                DateTimeOffset.UtcNow));
        }
    }

    private sealed class FakeRepository : ITrackedProductRepository
    {
        private readonly List<TrackedProduct> products = [];
        public int SaveChangesCalls { get; private set; }

        public Task<TrackedProduct?> FindBySkuAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(products.SingleOrDefault(product => product.Sku == sku));
        }

        public Task<IReadOnlyCollection<TrackedProduct>> ListAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<TrackedProduct>>(products);
        }

        public Task AddAsync(
            TrackedProduct product,
            CancellationToken cancellationToken = default)
        {
            products.Add(product);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }
    }
}
