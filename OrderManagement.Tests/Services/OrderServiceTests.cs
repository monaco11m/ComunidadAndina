using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using Xunit;

namespace OrderManagement.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderFactory> _factoryMock;
        private readonly Mock<IMessagePublisher> _publisherMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _factoryMock = new Mock<IOrderFactory>();
            _publisherMock = new Mock<IMessagePublisher>();

            _service = new OrderService(_unitOfWorkMock.Object,  _publisherMock.Object, _factoryMock.Object);
        }

        [Fact]
        public async Task CreateOrder_Should_Create_Order_Successfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Price = 10, Stock = 10 };

            var dto = new CreateOrderDto
            {
                CustomerName = "Elmer",
                CustomerEmail = "elmer@test.com",
                Items = new List<CreateOrderItemDto>
                {
                    new() { ProductId = productId, Quantity = 1 }
                }
            };

            var order = new Order
            {
                Id = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new() { ProductId = productId, Quantity = 1, UnitPrice = 10 }
                }
            };

            _factoryMock.Setup(f => f.CreateOrder(dto)).Returns(order);
            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(productId)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.Orders.AddAsync(order)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateOrderAsync(dto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Orders.AddAsync(order), Times.Once);
            result.Should().Be(order.Id);
        }

        [Fact]
        public async Task CreateOrder_Should_Throw_Exception_When_Stock_Is_Insufficient()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "Elmer",
                CustomerEmail = "elmer@test.com",
                Items = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 10 }
                }
            };

            var product = new Product
            {
                Id = dto.Items.First().ProductId,
                Name = "Test Product",
                Price = 50,
                Stock = 5 
            };

            _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            _factoryMock.Setup(f => f.CreateOrder(dto))
                .Returns(new Order());

            // Act
            Func<Task> act = async () => await _service.CreateOrderAsync(dto);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>();

            _unitOfWorkMock.Verify(u => u.Orders.AddAsync(It.IsAny<Order>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelOrder_Should_Update_Status_To_Cancelled()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Pending
            };

            _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _unitOfWorkMock.Setup(u => u.Orders.UpdateAsync(order))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CancelOrderAsync(orderId);

            // Assert
            result.Status.Should().Be("Cancelled");
            _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelOrder_Should_Throw_When_Order_Is_Completed()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                Status = OrderStatus.Completed
            };

            _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            Func<Task> act = async () => await _service.CancelOrderAsync(orderId);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>();

            _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(It.IsAny<Order>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }


    }

}
