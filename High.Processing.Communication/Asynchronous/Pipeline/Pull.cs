using High.Processing.Communication.Asynchronous.Abstract;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Asynchronous.Pipeline;

public class PipelineReceiver(string host, int port) : Receiver
{
    private readonly string _address = $"tcp://{host}:{port}";

    protected override INetMQSocket CreateSocket(string topic)
    {
        var receiver = new PullSocket();
        receiver.Connect(_address);
        return receiver;
    }
}