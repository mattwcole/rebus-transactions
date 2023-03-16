using Rebus.Bus;
using Rebus.Handlers;
using RebusTransactions.Messages;

namespace RebusTransactions;

public class MyHandler : IHandleMessages<InboundMessage>
{
    private readonly IBus _bus;

    public MyHandler(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task Handle(InboundMessage message)
    {
        await _bus.Publish(new OutboundMessage
        {
            Bar = message.Foo
        });

        // Uncommenting this prevents the outbound message from sending
        // throw new Exception("Failure after publish.");
    }
}