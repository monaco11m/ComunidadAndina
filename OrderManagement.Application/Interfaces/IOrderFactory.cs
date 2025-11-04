
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces
{
    public interface IOrderFactory
    {
        Order CreateOrder(CreateOrderDto dto);
    }
}
