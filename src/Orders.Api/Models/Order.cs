namespace Orders.Api.Models;

public sealed record Order(
    Guid Id,
    string CustomerName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice,
    DateTimeOffset CreatedAtUtc);

public sealed record CreateOrderRequest(
    string CustomerName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice);
