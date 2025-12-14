using Microsoft.AspNetCore.Mvc.Formatters;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services;

internal class OrderService(IOrder orderInterface,
    HttpClient httpClient,
    ResiliencePipelineProvider<string> resiliencePipeline /*This is for polly retries*/) : IOrderService
{
    // Get Product
    private async Task<ProductDTO> GetProduct(int id)
    {
        // Call product api using http client
        // Redirect this call to the api gateway since product api is not response to outsiders.
        var getProduct = await httpClient.GetAsync($"/api/products/{id}");

        if(!getProduct.IsSuccessStatusCode)
            return null!;

        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }

    // Get user
    private async Task<AppUserDTO> GetUser(int id)
    {
        // Call user api using http client
        // Redirect this call to the api gateway since user api is not response to outsiders.
        var getUser = await httpClient.GetAsync($"/api/authentication/{id}");
        if (!getUser.IsSuccessStatusCode)
            return null!;

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }


    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order == null || order.Id <= 0)
            return null!;

        // Get Retry pipeline
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        // Get Product
        var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

        // Get User
        var userDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

        // populate order details 
        return new OrderDetailsDTO(
            orderId,
            productDto.Id,
            userDto.Id,
            userDto.Name,
            userDto.Email,
            userDto.Address,
            userDto.TelephoneNumber,
            productDto.Name,
            order.PurchaseQuantity,
            productDto.Price,
            productDto.Price * order.PurchaseQuantity, 
            order.OrderedDate
        );
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
    {
        var orders = await orderInterface.GetOrderAsync(o => o.ClientId == clientId);
        if (orders == null || !orders.Any())
            return null!;

        var (_,ordersDtos) = OrderConversion.FromEntity(null, orders);

        return ordersDtos!;
    }
}
