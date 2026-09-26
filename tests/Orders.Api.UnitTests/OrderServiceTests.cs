using Orders.Api.Models;
using Orders.Api.Services;

namespace Orders.Api.UnitTests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_persists_valid_order()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository);
        var request = new CreateOrderRequest("Acme Corp", "SKU-100", 2, 19.99m);

        var created = await service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Acme Corp", created.CustomerName);
        var listed = await service.ListAsync();
        Assert.Single(listed);
    }

    [Theory]
    [InlineData("", "SKU-1", 1, 1)]
    [InlineData("Customer", "", 1, 1)]
    [InlineData("Customer", "SKU-1", 0, 1)]
    [InlineData("Customer", "SKU-1", 1, -1)]
    public void CreateAsync_rejects_invalid_request(
        string customer,
        string sku,
        int quantity,
        decimal unitPrice)
    {
        var service = new OrderService(new InMemoryOrderRepository());
        var request = new CreateOrderRequest(customer, sku, quantity, unitPrice);

        Assert.ThrowsAny<Exception>(() => service.CreateAsync(request).GetAwaiter().GetResult());
    }

    [Fact]
    public async Task DeleteAsync_returns_false_when_missing()
    {
        var service = new OrderService(new InMemoryOrderRepository());
        var deleted = await service.DeleteAsync(Guid.NewGuid());
        Assert.False(deleted);
    }
}
