using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OrderManagement.Application.Events;
using OrderManagement.Infrastructure.Services;
using Serilog;

namespace OrderManagement.Infrastructure.Messaging
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public RabbitMqConsumerService(IConfiguration config, EmailService emailService)
        {
            _config = config;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(messageJson);

                if (orderEvent != null)
                {
                    Log.Information("Received message from RabbitMQ for Order {OrderId}", orderEvent.OrderId);

                    await _emailService.SendOrderConfirmationEmailAsync(
                        orderEvent.CustomerEmail,
                        orderEvent.CustomerName,
                        orderEvent.OrderId,
                        orderEvent.TotalAmount
                    );

                    Log.Information("Confirmation email sent to {Email}", orderEvent.CustomerEmail);
                }
            };

            channel.BasicConsume(queue: "order-created-queue", autoAck: true, consumer: consumer);

            Log.Information("RabbitMQ consumer started. Listening to order-created-queue...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
