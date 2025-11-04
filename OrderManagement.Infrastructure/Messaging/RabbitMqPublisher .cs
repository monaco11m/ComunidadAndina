using Microsoft.Extensions.Configuration;
using OrderManagement.Application.Events;
using OrderManagement.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;


namespace OrderManagement.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IConfiguration _config;

        public RabbitMqPublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishAsync(OrderCreatedEvent evt)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQ:Host"],
                UserName = _config["RabbitMQ:Username"],
                Password = _config["RabbitMQ:Password"]
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
