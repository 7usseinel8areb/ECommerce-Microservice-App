using OrderApi.Application.DTOs;

namespace OrderApi.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId);
    Task<OrderDetailsDTO> GetOrderDetails(int orderId);

    Task<AppUserDTO> GetUser(int id);
    Task<ProductDTO> GetProduct(int id);
}
