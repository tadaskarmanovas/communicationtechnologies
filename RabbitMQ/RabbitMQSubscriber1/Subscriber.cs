using Data;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Constants = Data.Constants;

namespace RabbitMQSubscriber;

public class Subscriber : IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public Subscriber(string exchangeName, ILogger logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = Constants.RabbitMQHostName,
            UserName = Constants.RabbitMQUserName,
            Password = Constants.RabbitMQPassword,
            VirtualHost = Constants.RabbitMQVirtualHost,
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName,
            exchange: exchangeName,
            routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ConsumerOnReceived(logger);
        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    private static EventHandler<BasicDeliverEventArgs> ConsumerOnReceived(ILogger logger)
    {
        return (_, ea) =>
        {
            var receivedTime = DateTime.Now;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString((byte[])body);
            var textModel = JsonSerializer.Deserialize<TextModel>(message);
            logger.LogInformation($"Message {textModel?.Text[..10]} received. Time {receivedTime:hh:mm:ss.fff}");
        };
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }
}