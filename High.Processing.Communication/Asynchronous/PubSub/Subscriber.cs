using High.Processing.Communication.Asynchronous.Abstract;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Asynchronous.PubSub;

public class Subscriber(string host, int port) : Receiver
{

    private readonly string _address = $"tcp://{host}:{port}";

    protected override INetMQSocket CreateSocket(string topic)
    {
        var subscriber = new SubscriberSocket();
        subscriber.Connect(_address);
        subscriber.Subscribe(topic);
        return subscriber;
    }
}