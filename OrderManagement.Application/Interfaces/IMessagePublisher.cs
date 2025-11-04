using OrderManagement.Application.Events;

namespace OrderManagement.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(OrderCreatedEvent evt);
    }
}
