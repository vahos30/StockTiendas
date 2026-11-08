using StockTiendas.Application.Products;
using StockTiendas.Domain.Products;

namespace StockTiendas.Tests.Products;

public sealed class ListProductsServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithoutRefresh_DoesNotQueryProvider()
    {
        var product = new TrackedProduct
        {
            Sku = "68821-40094254606",
            Name = "Sanduchera Hamilton Beach",
            CreatedAt = DateTimeOffset.UtcNow,
            LastCheckedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            Availability =
            [
                new TrackedProductClubAvailability
                {
                    ClubCode = "6106",
                    ClubName = "Las Américas",
                    IsAvailable = true,
                    AvailableQuantity = 10,
                    CheckedAt = DateTimeOffset.UtcNow,
                }
            ]
        };

        var repository = new FakeRepository([product]);
        var provider = new FakeProvider();
        var service = new ListProductsService(repository, provider);

        var result = await service.ExecuteAsync(new ListProductsQuery(), refresh: false);

        Assert.Single(result);
        Assert.Equal(0, provider.CallCount);
        Assert.Equal(10, result.First().Availability.First().AvailableQuantity);
    }

    [Fact]
    public async Task ExecuteAsync_WithRefresh_UpdatesAvailabilityFromProviderAndSavesChanges()
    {
        var product = new TrackedProduct
        {
            Sku = "68821-40094254606",
            Name = "Sanduchera Hamilton Beach",
            CreatedAt = DateTimeOffset.UtcNow,
            LastCheckedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Availability =
            [
                new TrackedProductClubAvailability
                {
                    ClubCode = "6106",
                    ClubName = "Las Américas",
                    IsAvailable = true,
                    AvailableQuantity = 10,
                    CheckedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
                }
            ]
        };

        var repository = new FakeRepository([product]);
        var provider = new FakeProvider(new ProductAvailability(
            "68821-40094254606",
            "Sanduchera Hamilton Beach",
            true,
            [
                new StoreAvailability("6106", "Las Américas", true, 91),
                new StoreAvailability("6110", "El Poblado", true, 39)
            ],
            DateTimeOffset.UtcNow));
        var service = new ListProductsService(repository, provider);

        var result = await service.ExecuteAsync(new ListProductsQuery(), refresh: true);

        Assert.Single(result);
        Assert.Equal(2, result.First().Availability.Count);
        Assert.Equal(91, result.First().Availability.First(av => av.ClubCode == "6106").AvailableQuantity);
        Assert.Equal(39, result.First().Availability.First(av => av.ClubCode == "6110").AvailableQuantity);
        Assert.Equal(1, repository.SaveChangesCalls);
        Assert.Equal(1, provider.CallCount);
    }

    private sealed class FakeProvider(ProductAvailability? result = null) : IProductAvailabilityProvider
    {
        public int CallCount { get; private set; }

        public Task<ProductAvailability> GetAvailabilityAsync(
            Uri productUri,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(result ?? new ProductAvailability(
                "68821-40094254606",
                "Sanduchera Hamilton Beach",
                true,
                [],
                DateTimeOffset.UtcNow));
        }
    }

    private sealed class FakeRepository(List<TrackedProduct> products) : ITrackedProductRepository
    {
        public int SaveChangesCalls { get; private set; }

        public Task<TrackedProduct?> FindBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(products.SingleOrDefault(product => product.Sku == sku));
        }

        public Task<IReadOnlyCollection<TrackedProduct>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<TrackedProduct>>(products);
        }

        public Task AddAsync(TrackedProduct product, CancellationToken cancellationToken = default)
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
