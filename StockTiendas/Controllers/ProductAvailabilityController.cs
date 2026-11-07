using Microsoft.AspNetCore.Mvc;
using StockTiendas.Application.Products;

namespace StockTiendas.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductAvailabilityController(
    GetProductAvailabilityService availabilityService) : ControllerBase
{
    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] string? url,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(new { message = "El parámetro 'url' es obligatorio." });
        }

        try
        {
            var result = await availabilityService.ExecuteAsync(
                new GetProductAvailabilityQuery(url),
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                detail: "No fue posible consultar el producto en la tienda externa.",
                statusCode: StatusCodes.Status502BadGateway,
                title: exception.Message);
        }
    }
}
