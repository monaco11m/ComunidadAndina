

namespace OrderManagement.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
