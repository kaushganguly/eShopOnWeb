using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoint;

namespace Microsoft.eShopWeb.PublicApi;

public static class EndpointRegistrationExtensions
{
    public static IServiceCollection AddPublicApiEndpoints(this IServiceCollection services)
    {
        var endpointTypes = typeof(Program).Assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var endpointType in endpointTypes)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapPublicApiEndpoints(this IEndpointRouteBuilder app)
    {
        foreach (var endpoint in app.ServiceProvider.GetServices<IEndpoint>())
        {
            endpoint.AddRoute(app);
        }

        return app;
    }
}
