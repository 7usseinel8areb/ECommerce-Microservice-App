using Ocelot.DependencyInjection;
using eCommerce.SharedLibrary.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Microsoft.AspNetCore.Authentication;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;
var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json",optional: false, reloadOnChange:true);
builder.Services.AddOcelot()
    .AddCacheManager(x => x.WithDictionaryHandle());

JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });

});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.UseMiddleware<AttachSignatureToRequest>(); 
app.UseOcelot().Wait();

app.Run();