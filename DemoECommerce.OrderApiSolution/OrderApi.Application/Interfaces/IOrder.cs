using eCommerce.SharedLibrary.Interface;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;

namespace OrderApi.Application.Interfaces;

public interface IOrder : IGenericInterface<Order>
{
    Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order,bool>> expression);
}
