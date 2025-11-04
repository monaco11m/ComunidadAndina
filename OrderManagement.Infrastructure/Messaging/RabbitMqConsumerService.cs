using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrderManagement.Application.Events;
using OrderManagement.Infrastructure.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Infrastructure.Messaging
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly EmailService _emailService;
        private readonly RabbitMqSettings _settings;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqConsumerService(IOptions<RabbitMqSettings> settings, EmailService emailService)
        {
            _settings = settings.Value;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            int retries = 8;
            while (retries > 0 && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _connection = factory.CreateConnection(new List<AmqpTcpEndpoint>
                    {
                        new AmqpTcpEndpoint(_settings.Host, _settings.Port)
                    });

                    break; 
                }
                catch (Exception ex)
                {
                    retries--;
                    Log.Warning("RabbitMQ no ready yet ({Message}). Trying again... ({Retries} tries left)", ex.Message, retries);
                    await Task.Delay(3000, stoppingToken);
                }
            }

            if (_connection == null)
            {
                Log.Error("Not possible to connect after 8 chances.");
                return;
            }

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "order-created-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(messageJson);

                if (orderEvent != null)
                {
                    Log.Information("Message received for order {OrderId}", orderEvent.OrderId);

                    await _emailService.SendOrderConfirmationEmailAsync(
                        orderEvent.CustomerEmail,
                        orderEvent.CustomerName,
                        orderEvent.OrderId,
                        orderEvent.TotalAmount
                    );

                    Log.Information("Email sent to {Email}", orderEvent.CustomerEmail);
                }
            };

            _channel.BasicConsume(
                queue: "order-created-queue",
                autoAck: true,
                consumer: consumer);

            Log.Information("RabbitMQ Consumer ready...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
