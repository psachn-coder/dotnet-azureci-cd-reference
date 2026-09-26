using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Orders.Api.Models;

namespace Orders.Api.IntegrationTests;

public class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrdersApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_and_get_order_roundtrip()
    {
        var request = new CreateOrderRequest("Contoso", "SKU-42", 3, 12.5m);
        var createResponse = await _client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/orders/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<Order>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Contoso", fetched.CustomerName);
    }

    [Fact]
    public async Task Invalid_create_returns_bad_request()
    {
        var request = new CreateOrderRequest("", "SKU-1", 1, 1m);
        var response = await _client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.True(document.RootElement.TryGetProperty("error", out _));
    }
}
