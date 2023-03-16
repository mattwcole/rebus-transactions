using Polly;
using Rebus.Activation;
using Rebus.Config;
using RebusTransactions.Messages;
using Xunit;

namespace RebusTransactions.Tests;

public class TransactionTest
{
    [Fact]
    public async Task Test()
    {
        OutboundMessage? handledMessage = null;

        var handlerActivator = new BuiltinHandlerActivator();
        handlerActivator.Handle<OutboundMessage>(m =>
        {
            handledMessage = m;
            return Task.CompletedTask;
        });

        using var bus = Configure
            .With(handlerActivator)
            .Transport(transport => transport.UseRabbitMq("amqp://guest:guest@localhost", "MyTests"))
            .Start();

        await bus.Subscribe<OutboundMessage>();

        // Send inbound message to the service
        await bus.Publish(new InboundMessage
        {
            Foo = "hello"
        });

        // Wait for outbound message from the service
        await Policy
            .HandleResult<OutboundMessage?>(m => m is null)
            .WaitAndRetryAsync(20, _ => TimeSpan.FromSeconds(0.5))
            .ExecuteAsync(() => Task.FromResult(handledMessage));

        Assert.NotNull(handledMessage);
        Assert.Equal("hello", handledMessage.Bar);
    }
}