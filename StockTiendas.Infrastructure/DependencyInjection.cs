using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StockTiendas.Domain.Products;
using StockTiendas.Infrastructure.Persistence;
using StockTiendas.Infrastructure.PriceSmart;

namespace StockTiendas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<StockTiendasDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<ITrackedProductRepository, TrackedProductRepository>();
        services.AddOptions<PriceSmartOptions>();
        services.AddHttpClient<IProductAvailabilityProvider, PriceSmartAvailabilityProvider>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("StockTiendas/1.0");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        return services;
    }
}
