using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories;

internal class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try
        {
            var order = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            
            return order.Id > 0? new Response(true, "Order created successfully.") 
                               : new Response(false, "Failed to create order.");
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            return new Response(false, "An error occurred while creating this order. Please try again.");
        }
    }

    public async Task<Response> DeleteAsync(int id)
    {
        try
        {
            var order = await FindByIdAsync(id);
            if (order is null)
            {
                return new Response(false, "Order not found");
            }

            context.Orders.Remove(order);
            await context.SaveChangesAsync();

            return new Response(true, "Order deleted successfully.");
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            return new Response(false, "An error occurred while deleting the product. Please try again.");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            var order = await context.Orders.FindAsync(id);
            return order is null ? null! : order;
        }
        catch (Exception ex)
            {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            throw new Exception("An error occurred while retriving this order. Please try again.");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            var orders = await context.Orders
                .AsNoTracking()
                .ToListAsync();

            return orders is null ? null! : orders;

        }
        catch (Exception ex)
            {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            throw new Exception("An error occurred while retriving orders. Please try again.");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> expression)
    {
        try
        {
            var order = await context.Orders
                .Where(expression)
                .FirstOrDefaultAsync();

            return order is null ? null! : order;
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            throw new Exception("An error occurred while retriving this order. Please try again.");
        }
    }

    public async Task<IEnumerable<Order>> GetOrderAsync(Expression<Func<Order, bool>> expression)
    {
        try
        {
            var orders = await context.Orders
                .Where(expression)
                .ToListAsync();

            return orders is null ? null! : orders;
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            throw new Exception("An error occurred while retriving this order. Please try again.");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order is null)
            {
                return new Response(false, "Order not found");
            }

            context.Entry(order).State = EntityState.Detached;
            context.Orders.Update(entity);
            await context.SaveChangesAsync();

            return new Response(true, "Order updated successfully.");
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // display scary-free message to user
            return new Response(false, "An error occurred while updating this order. Please try again.");
        }
    }
}
