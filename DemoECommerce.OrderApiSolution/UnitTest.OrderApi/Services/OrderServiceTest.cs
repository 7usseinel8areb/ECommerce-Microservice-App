using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services;

public class OrderServiceTest
{
    private readonly IOrderService _orderService;
    private readonly IOrder _order;
    public OrderServiceTest()
    {
        _orderService = A.Fake<IOrderService>();
        _order = A.Fake<IOrder>();
    }


    // Create a fake HttpMessageHandler to mock HttpClient responses
    public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response = response;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    // Create a fake HttpClient using the FakeHttpMessageHandler
    private static HttpClient CreateFakeHttpClient(object options)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(options)
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
        var httpClient = new HttpClient(fakeHttpMessageHandler)
        {
            BaseAddress = new Uri("http://localhost"),

        };
        return httpClient;
    }

    // Get Product
    [Fact]
    public async Task GetProducts_ValidProductId_ReturnsProduct()
    {
        // Arrange //
        int productId = 1;
        var productDto = new ProductDTO(productId, "Test Product", 10, 99.99m);
        var httpClient = CreateFakeHttpClient(productDto);

        // System Under Test - SUT
        // We need only HttpClient for this test to make calls
        // Specify other dependencies as null at OrderService constructor
        var orderService = new OrderService(null!, httpClient, null!);

        // Act //
        var result = await orderService.GetProduct(productId);

        // Assert //
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductDTO>();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetProducts_InvalidProductId_ReturnsNull()
    {
        // Arrange //
        int productId = -1; // Invalid product ID
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
        var httpClient = CreateFakeHttpClient(null!);

        // System Under Test - SUT
        // We need only HttpClient for this test to make calls
        // Specify other dependencies as null at OrderService constructor
        var orderService = new OrderService(null!, httpClient, null!);

        // Act //
        var result = await orderService.GetProduct(productId);

        // Assert //
        result.Should().BeNull();
    }


    // Get Client Order By Id
    [Fact]
    public async Task GetOrdersByClientId_OrderExists_ReturnsOrderDetails()
    {
        // Arrange //
        int clientId = 1;
        var orders = new List<Order>()
        {
            new (){ Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow },
            new (){ Id = 2, ProductId = 2, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow }
        };

        A.CallTo(() => _order.GetOrdersAsync
        (A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
        var orderService = new OrderService(_order, null!, null!);
        // Act //
        var result = await orderService.GetOrdersByClientId(clientId);

        // Assert //
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(orders.Count);
    }
}
