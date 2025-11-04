using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests.TestHelpers;

public static class OrderTestHelper
{
    public static Order CreateValidOrder(Guid? id = null)
    {
        return new Order
        {
            Id = id ?? Guid.NewGuid(),
            CustomerName = "Elmer Customer",
            CustomerEmail = "elmercustomer@test.com",
            OrderNumber = "ORD-TEST-001",
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 100m,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 1,
                    UnitPrice = 100m
                }
            }
        };
    }
}
