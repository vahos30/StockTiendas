using Microsoft.EntityFrameworkCore;
using StockTiendas.Application.Products;
using StockTiendas.Infrastructure;
using StockTiendas.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendLocal", policy =>
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GetProductAvailabilityService>();
builder.Services.AddScoped<RegisterProductService>();
builder.Services.AddScoped<ListProductsService>();


builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se configuró la conexión DefaultConnection."));

var app = builder.Build();

// Aplicar migraciones y crear la base de datos automáticamente al arrancar
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockTiendasDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendLocal");

app.UseAuthorization();

app.MapControllers();

app.Run();
