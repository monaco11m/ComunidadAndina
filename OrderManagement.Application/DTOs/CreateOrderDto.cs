

namespace OrderManagement.Application.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
