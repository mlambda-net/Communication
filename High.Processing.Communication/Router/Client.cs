using System.Text;
using System.Text.Json;
using High.Processing.Communication.Router.Protocol;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Router;

public class Client : IDisposable
{
    private readonly string _address;
    private readonly CancellationTokenSource _cancellation;
    private readonly DealerSocket _dealer;
    private readonly Thread _listener;
    private readonly Dictionary<string, List<Action<string>>> _handlers;
    private readonly Guid _id;

    public Client(string host, int port)
    {
        _address = $"tcp://{host}:{port}";
        _id = Guid.NewGuid();
        _dealer = new DealerSocket();
        _handlers = new Dictionary<string, List<Action<string>>>();
        _cancellation = new CancellationTokenSource();
        _dealer.Options.Identity = Encoding.UTF8.GetBytes(_id.ToString());
        _dealer.Connect(_address);
        _listener = new Thread(Listen);
    }

    public void Dispose()
    {
        _dealer.Dispose();
        _cancellation.Dispose();
    }


    public void Connect()
    {
        _listener.Start();
        Console.WriteLine($"{_id} connected to router...");
    }

    public void Disconnect()
    {
        _cancellation.Cancel();
    }


    public Task Subscribe<T>(string address, Action<T> handler)
    {
        return Task.Run(() =>
        {
            var info = JsonSerializer.Serialize(new ClientInfo(_id, "Address"));
            var data = JsonSerializer.Serialize(new Address(address, _id));
            _dealer.SendMoreFrame(info).SendFrame(data);
            var ackMsg = _dealer.ReceiveFrameString();
            var ack = JsonSerializer.Deserialize<Acknowledge>(ackMsg);
            if (ack == null) return;
            var transform = (string received) =>
            {
                var msg = JsonSerializer.Deserialize<T>(received);
                if (msg != null) handler(msg);
            };

            if (_handlers.ContainsKey(info))
                _handlers[address].Add(transform);
            else
                _handlers.Add(address, [transform]);
        });
    }


    public Task Send<T>(string address, T data)
    {
        return Task.Run(() =>
        {
            var info = JsonSerializer.Serialize(new ClientInfo(_id, "Message"));
            var content = JsonSerializer.Serialize(data);
            var message = new Message(content, address);
            var json = JsonSerializer.Serialize(message);
            _dealer.SendMoreFrame(info).SendFrame(json);
        });
    }

    private void Listen()
    {
        try
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var data = _dealer.ReceiveFrameString();

                var msg = JsonSerializer.Deserialize<Message>(data);
                if (msg != null && _handlers.TryGetValue(msg.Address, out var handlers))
                    handlers.AsParallel().ForAll(handler => handler(msg.Content));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Graceful exit
            Console.WriteLine("Message handling was canceled.");
        }
    }
}