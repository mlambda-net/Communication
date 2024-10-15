using High.Processing.Communication.Synchronous.Abstract;
using NetMQ.Sockets;

namespace High.Processing.Communication.Synchronous.Socket;

public class Client : IClient
{
    private readonly string _address;
    private readonly Pipeline _pipeline;
    private readonly RequestSocket _socket;

    public Client(string host, int port)
    {
        _address = $"tcp://{host}:{port}";
        _socket = new RequestSocket();
        _pipeline = new Pipeline(_socket);
    }

    public async Task Send<T>(T message)
    {
        await _pipeline.Send(message);
    }

    public async Task Receive<T>(Action<T> callback)
    {
        await _pipeline.Receive(callback);
    }

    public void Connect()
    {
        _pipeline.Start();
        _socket.Connect(_address);
    }

    public void Disconnect()
    {
        _pipeline.Stop();
    }
}