using High.Processing.Communication.Asynchronous.Abstract;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Asynchronous.Pipeline;

public class Push : Sender
{
    private readonly PushSocket _sender;

    public Push(int port)
    {
        _sender = new PushSocket();
        _sender.Bind($"tcp://*:{port}");
    }

    protected override INetMQSocket Socket => _sender;
}