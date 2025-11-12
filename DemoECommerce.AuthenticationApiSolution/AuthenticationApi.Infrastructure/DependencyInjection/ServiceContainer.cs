using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Services.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Infrastructure.Services;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {

        // Add db connectivity
        // Jwt Add auth scheme
        SharedService.AddSharedServices<AuthenticationDbContext>(services, configuration, configuration["MySerilog:FileName"]!);

        // Create Depedency injection for Repositories
        services.AddScoped<IUser, UserRepository>();

        // Create Depedency injection for Services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }


    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Register middlewares such as:
        // Global exception
        // Listen to only api gateway calls
        SharedService.UseSharedPolicies(app);
        return app;
    }
}
