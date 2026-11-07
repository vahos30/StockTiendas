using Microsoft.AspNetCore.Mvc;
using StockTiendas.Application.Products;

namespace StockTiendas.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    RegisterProductService registerProductService,
    ListProductsService listProductsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await registerProductService.ExecuteAsync(
                new RegisterProductCommand(request.Name, request.Sku),
                cancellationToken);

            return Ok(product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                detail: "No fue posible consultar la disponibilidad en PriceSmart.",
                statusCode: StatusCodes.Status502BadGateway,
                title: exception.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<object>>> List(
        CancellationToken cancellationToken)
    {
        var products = await listProductsService.ExecuteAsync(
            new ListProductsQuery(),
            cancellationToken);

        return Ok(products);
    }
}

public sealed record RegisterProductRequest(string Name, string Sku);
