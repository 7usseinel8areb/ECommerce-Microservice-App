using eCommerce.SharedLibrary.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class SharedService
{
    public static IServiceCollection AddSharedServices<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string fileName) where TContext : DbContext
    {
        // Add Geeneric Database context
        services.AddDbContext<TContext>(
            option => option.UseSqlServer(configuration.GetConnectionString("eCommerceConnection"),
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
            ));

        // Config Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            /*
             ده معناه إن الـ logger هيبدأ يسجل من مستوى Information وأعلى
            (يعني هيشمل: Information, Warning, Error, Fatal).
            لكن مش هيسجل Debug أو Verbose لأنهم أقل.
             */
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
                          restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,// restrictedToMinimumLevel: بيقول إن الملف هيسجل فقط اللوجات اللي من Information وأعلى.
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}", // 2025-10-27 10:15:32.456 +02:00 [INF] Application started successfully
                          rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add Jwt auth scheme
        JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // use global exceptions
        app.UseMiddleware<GlobalException>();

        // Middleware to block all outsiders api calls
        app.UseMiddleware<ListenToOnlyApiGateway>();

        return app;
    }
}
