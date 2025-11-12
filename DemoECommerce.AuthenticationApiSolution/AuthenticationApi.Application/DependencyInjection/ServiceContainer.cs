using AuthenticationApi.Application.Services.Implementation;
using AuthenticationApi.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all services in Application layer
        //services.Scan(scan => scan
        //    .FromAssemblyOf<ApplicationServiceContainer>()
        //    .AddClasses(classes => classes.InNamespaces("AuthenticationApi.Application.Services"))
        //    .AsImplementedInterfaces()
        //    .WithScopedLifetime());

        services.AddScoped<IUserService, UserService>();

        return services;
    }

}
