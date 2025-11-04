
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Factories
{
    public class OrderFactory : IOrderFactory
    {
        public Order CreateOrder(CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                TotalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity)
            };

            return order;
        }
    }
}
