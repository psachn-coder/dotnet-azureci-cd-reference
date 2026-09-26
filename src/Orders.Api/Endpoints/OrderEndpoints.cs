using Orders.Api.Models;
using Orders.Api.Services;

namespace Orders.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/", async (OrderService service, CancellationToken ct) =>
        {
            var orders = await service.ListAsync(ct);
            return Results.Ok(orders);
        })
        .WithName("ListOrders")
        .WithSummary("List all orders");

        group.MapGet("/{id:guid}", async (Guid id, OrderService service, CancellationToken ct) =>
        {
            var order = await service.GetAsync(id, ct);
            return order is null ? Results.NotFound() : Results.Ok(order);
        })
        .WithName("GetOrderById")
        .WithSummary("Get an order by id");

        group.MapPost("/", async (CreateOrderRequest request, OrderService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/orders/{created.Id}", created);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateOrder")
        .WithSummary("Create a new order");

        group.MapDelete("/{id:guid}", async (Guid id, OrderService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteOrder")
        .WithSummary("Delete an order");

        return endpoints;
    }
}
