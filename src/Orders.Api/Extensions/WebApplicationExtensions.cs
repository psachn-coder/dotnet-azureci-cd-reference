using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;

namespace Orders.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddOrdersApiConfiguration(this WebApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["KeyVault:Uri"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }

        return builder;
    }

    public static WebApplicationBuilder AddOrdersObservability(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? "orders-api";
        var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

        var telemetry = builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName));

        if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
        {
            telemetry.UseAzureMonitor();
        }

        return builder;
    }
}
