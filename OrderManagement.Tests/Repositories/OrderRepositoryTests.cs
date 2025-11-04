using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;
using OrderManagement.Tests.TestHelpers;
using Xunit;

namespace OrderManagement.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _repository;

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new AppDbContext(options);
            _repository = new OrderRepository(_context);
        }

        [Fact]
        public async Task Should_Add_And_Get_Order()
        {
            // Arrange
            var order = OrderTestHelper.CreateValidOrder();

            // Act
            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(order.Id);

            // Assert
            result.Should().NotBeNull();
            result!.CustomerName.Should().Be("Elmer Customer"); // like the helper
        }
    }
}
