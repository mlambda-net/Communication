using High.Processing.Communication.Synchronous.Abstract;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Synchronous.Socket;

public class Server: IServer
{
    private readonly Pipeline _pipeline;

    public Server(int port)
    {
        var socket = new ResponseSocket();
        socket.Bind($"tcp:*:{port}");
        _pipeline = new Pipeline(socket);
    }

    public async Task Send<T>(T message)
    {
      await _pipeline.Send(message);
    }

    public async Task Receive<T>(Action<T> callback)
    {
        await _pipeline.Receive<T>(callback);
    }

    public void Start()
    {
        _pipeline.Start();
    }

    public void Stop()
    {
        _pipeline.Stop();
    }
}