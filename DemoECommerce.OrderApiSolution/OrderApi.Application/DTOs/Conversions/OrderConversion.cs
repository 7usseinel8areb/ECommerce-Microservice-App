using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions;

public static class OrderConversion
{
    public static Order ToEntity(this OrderDTO dto) => new ()
    {
        Id = dto.Id,
        ProductId = dto.ProductId,
        ClientId = dto.ClientId,
        PurchaseQuantity = dto.PurchaseQuantity,
        OrderedDate = dto.OrderedDate
    };

    public static (OrderDTO?,IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
    {
        if (order is null && orders is not null)
        {
            var orderDTOs = orders.Select(o => new OrderDTO(
                o.Id,
                o.ProductId,
                o.ClientId,
                o.PurchaseQuantity,
                o.OrderedDate
            ));

            return (null, orderDTOs);
        }
        else if (order is not null && orders is null)
        {
            var orderDTO = new OrderDTO(
                order.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderedDate
            );

            return (orderDTO, null);
        }

        return (null, null);
    }
}
