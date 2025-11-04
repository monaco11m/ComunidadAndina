
namespace OrderManagement.Application.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerEmail { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
