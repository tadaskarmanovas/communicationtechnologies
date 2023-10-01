using Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQPublisher;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddHostedService<Worker>();
            })
            .Build()
            .Run();
    }
}

public class Worker : IHostedService, IDisposable
{
    private readonly Publisher _publisher;

    public Worker(ILogger<Worker> logger)
    {
        _publisher = new("10", logger);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _publisher.Publish(new TextModel { Text = Guid.NewGuid().ToString()[..10] });

        for (int i = 0; i < 10000; i++)
        {
            Task.Run(() => _publisher.Publish(new TextModel { Text = Guid.NewGuid().ToString()[..10] }), cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _publisher?.Dispose();
    }
}