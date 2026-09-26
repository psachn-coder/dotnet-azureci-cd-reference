using Orders.Api.Endpoints;
using Orders.Api.Extensions;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddJsonConsole();
}

builder.AddOrdersApiConfiguration();
builder.AddOrdersObservability();

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<OrderService>();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Orders API", Version = "v1" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API v1");
    options.RoutePrefix = "swagger";
});

app.MapHealthChecks("/health");
app.MapOrderEndpoints();

app.Run();

public partial class Program;
