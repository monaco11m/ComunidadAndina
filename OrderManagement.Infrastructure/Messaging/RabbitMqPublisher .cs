using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OrderManagement.Application.Events;
using OrderManagement.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;


namespace OrderManagement.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task PublishAsync(OrderCreatedEvent evt)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "order-created-queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var json = JsonSerializer.Serialize(evt);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: "order-created-queue",
                                 basicProperties: null,
                                 body: body);

            return Task.CompletedTask;
        }
    }
}
