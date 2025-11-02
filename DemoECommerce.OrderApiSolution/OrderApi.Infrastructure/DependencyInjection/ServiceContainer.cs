using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database connectivity 
        // Add Authentication scheme
        SharedService.AddSharedServices<OrderDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

        services.AddScoped<IOrder, OrderRepository>();
        return services;
    }

    public static IApplicationBuilder UseInfratstructure(this IApplicationBuilder app)
    {
        // Use Shared Policies
        // Register Middlewares such as handle external errors, logging, etc.
        // Listen to only api gateway calls 
        SharedService.UseSharedPolicies(app);

        return app;
    }
}
