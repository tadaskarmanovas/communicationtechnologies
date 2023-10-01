using Data;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Constants = Data.Constants;

namespace RabbitMQPublisher;

public class Publisher : IDisposable
{
    private readonly IModel? _channel;
    private readonly string _exchangeName;
    private readonly ILogger _logger;
    private readonly IConnection? _connection;

    public Publisher(string exchangeName, ILogger logger)
    {
        _exchangeName = exchangeName;
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = Constants.RabbitMQHostName,
            UserName = Constants.RabbitMQUserName,
            Password = Constants.RabbitMQPassword,
            VirtualHost = Constants.RabbitMQVirtualHost,
        };
        ;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
    }

    public void Publish(ITextModel textModel)
    {
        var chars = JsonSerializer.Serialize(textModel);
        var body = Encoding.UTF8.GetBytes(chars);

        _logger.LogInformation($"Message {textModel.Text[..10]} sent. Time: {DateTime.Now:hh:mm:ss.fff}");
        _channel.BasicPublish(_exchangeName, "", true, body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}