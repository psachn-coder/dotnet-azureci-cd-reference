using System.Collections.Concurrent;
using Orders.Api.Models;

namespace Orders.Api.Services;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Order> snapshot = _orders.Values
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<Order> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var order = new Order(
            Guid.NewGuid(),
            request.CustomerName.Trim(),
            request.ProductSku.Trim(),
            request.Quantity,
            request.UnitPrice,
            DateTimeOffset.UtcNow);

        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException("Failed to persist order.");
        }

        return Task.FromResult(order);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_orders.TryRemove(id, out _));
    }
}
