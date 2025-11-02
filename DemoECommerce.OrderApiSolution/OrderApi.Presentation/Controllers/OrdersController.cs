using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController (IOrder orderInterface, IOrderService orderService): ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
            {
                return NotFound("No orders was found");
            }

            var (_, ordersList) = OrderConversion.FromEntity(null, orders);

            return Ok(ordersList);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
                return NotFound($"Order was not found");
            
            var (orderDTO, _) = OrderConversion.FromEntity(order, null);
            return Ok(orderDTO);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderService.GetOrdersByClientId(clientId);
            if (orders is null && !orders!.Any())
                return NotFound("No orders was found for the specified client.");

            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            var orderDetails = await orderService.GetOrderDetails(orderId);
            if (orderDetails is null)
                return NotFound("Order details not found.");

            return Ok(orderDetails);
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            // Check model state if all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            // Convert to entity
            var orderEntity = orderDTO.ToEntity();

            var response = await orderInterface.CreateAsync(orderEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            // Check model state if all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            // Convert to entity
            var orderEntity = orderDTO.ToEntity();

            var response = await orderInterface.UpdateAsync(orderEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var response = await orderInterface.DeleteAsync(id);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

    }
}
