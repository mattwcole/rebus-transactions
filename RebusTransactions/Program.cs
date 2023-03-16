using Rebus.Config;
using RebusTransactions;
using RebusTransactions.Messages;

const string rabbitConnection = "amqp://guest:guest@localhost";

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Subscribing bus
        services.AddRebus(
            configurer => configurer
                .Transport(transport => transport.UseRabbitMq(rabbitConnection, "MyApp")),
            isDefaultBus: false,
            async bus => await bus.Subscribe<InboundMessage>());

        // Publishing bus
        services.AddRebus(
            configurer => configurer
                .Transport(transport => transport.UseRabbitMqAsOneWayClient(rabbitConnection)),
            isDefaultBus: true);

        services.AddRebusHandler<MyHandler>();
    })
    .Build();

await host.RunAsync();
