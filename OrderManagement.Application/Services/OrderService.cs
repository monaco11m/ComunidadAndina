using OrderManagement.Application.DTOs;
using OrderManagement.Application.Events;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Interfaces;

namespace OrderManagement.Application.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessagePublisher _publisher;
        private readonly IOrderFactory _factory;

        public OrderService(IUnitOfWork unitOfWork, IMessagePublisher publisher, IOrderFactory factory)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _factory = factory;
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderDto dto)
        {
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with id {item.ProductId} not found.");

                if (product.Stock < item.Quantity)
                    throw new ValidationException(new[] { $"Insufficient stock for product {product.Name}." });

                product.Stock -= item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }

            var order = _factory.CreateOrder(dto);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CommitAsync();

            var evt = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount
            };
            await _publisher.PublishAsync(evt);

            return order.Id;
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Order with id {id} not found.");

            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

    }
}
