// See https://aka.ms/new-console-template for more information

using Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBomber.Contracts;
using NBomber.CSharp;
using RabbitMQPublisher;

Console.WriteLine("Hello, World!");

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();

var serviceProvider = serviceCollection.BuildServiceProvider();
var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("Logger");
var publisher = new Publisher("10", logger);

var scenario = Scenario.Create("RabbitMQ", async _ =>
    {
        await Task.Run(() => publisher.Publish(new TextModel { Text = Guid.NewGuid().ToString()[..10] }));

        return Response.Ok();
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(LoadSimulations());

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();

static LoadSimulation[] LoadSimulations()
{
    return new[]
    {
        Simulation.KeepConstant(10, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(20, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(30, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(40, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(50, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(60, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(70, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(80, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(90, TimeSpan.FromSeconds(30)),
        Simulation.KeepConstant(100, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(110, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(120, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(130, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(140, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(150, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(160, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(170, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(180, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(190, TimeSpan.FromSeconds(30)),
        // Simulation.KeepConstant(200, TimeSpan.FromSeconds(30)),

        //Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(30)),
        //Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromSeconds(30)),
        //Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30)),
        //Simulation.InjectPerSec(rate: 120, during: TimeSpan.FromSeconds(30)),
        //Simulation.InjectPerSec(rate: 500, during: TimeSpan.FromSeconds(30)),
        //Simulation.InjectPerSec(rate: 1000, during: TimeSpan.FromSeconds(30)),
    };
};