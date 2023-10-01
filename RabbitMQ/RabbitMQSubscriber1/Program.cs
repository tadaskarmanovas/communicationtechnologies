using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQSubscriber;

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
    private readonly Subscriber _subscriber;

    public Worker(ILogger<Worker> logger)
    {
        _subscriber = new Subscriber("10", logger);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _subscriber.Dispose();
    }
}