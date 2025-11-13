using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add microservices using relative paths from AppHost
builder.AddProject("auth", @"..\DemoECommerce.AuthenticationApiSolution\AuthenticationApi.Presentation\AuthenticationApi.Presentation.csproj");
builder.AddProject("products", @"..\DemoECommerce.ProductSolution\ProductApi.Presentation\ProductApi.Presentation.csproj");
builder.AddProject("orders", @"..\DemoECommerce.OrderApiSolution\OrderApi.Presentation\OrderApi.Presentation.csproj");


// Build and run all microservices
builder.Build().Run();
