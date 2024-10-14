using High.Processing.Communication.Asynchronous.Abstract;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Asynchronous.PubSub;

public class Publisher: Sender
{
    private readonly PublisherSocket _publisher;
    
    public Publisher(int port)
    {
        _publisher = new PublisherSocket();
        _publisher.Bind($"tcp://*:{port}");
    }
    protected override INetMQSocket Socket => _publisher;
}