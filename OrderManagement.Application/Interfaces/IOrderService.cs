using OrderManagement.Application.DTOs;


namespace OrderManagement.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<Guid> CreateOrderAsync(CreateOrderDto order);
        Task<OrderDto> CancelOrderAsync(Guid id);
    }
}
