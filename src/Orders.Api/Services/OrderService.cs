using Orders.Api.Models;

namespace Orders.Api.Services;

public sealed class OrderService(IOrderRepository repository)
{
    public Task<IReadOnlyList<Order>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);
        return repository.AddAsync(request, cancellationToken);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    internal static void ValidateCreateRequest(CreateOrderRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CustomerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ProductSku);

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Quantity must be greater than zero.");
        }

        if (request.UnitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Unit price cannot be negative.");
        }
    }
}
