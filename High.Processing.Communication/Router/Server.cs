using System.Text.Json;
using System.Text.Json.Nodes;
using High.Processing.Communication.Router.Protocol;
using NetMQ;
using NetMQ.Sockets;

namespace High.Processing.Communication.Router;

public class Server: IDisposable
{
    private readonly RouterSocket _router;
    private readonly Thread _listener;
    private readonly CancellationTokenSource _cancellation;
    private readonly Dictionary<string, Guid> _addresses;
    private readonly string _url;

    public Server(int port)
    {
        _router = new RouterSocket();
        _url = $"tcp://*:{port}";
        _listener = new Thread(Listen);
        _cancellation = new CancellationTokenSource();
        _addresses = new Dictionary<string, Guid>();
    }

    public void Start()
    {
        Console.WriteLine("Starting server...");
        _router.Bind(_url);
        _listener.Start();
    }

    public void Stop()
    {
        _cancellation.Cancel();
        _router.Close();
    }

    private void Listen()
    {
        try
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var identity = _router.ReceiveFrameString();
                var msg = _router.ReceiveFrameString();
                Console.WriteLine($"Received message: {msg} from client: {identity}");
                var info = JsonSerializer.Deserialize<ClientInfo>(msg);
                if (info != null)
                {
                    var message = _router.ReceiveFrameString();

                    switch (info.MessageKind)
                    {
                        case "Address":
                            Task.Run(() => AddressHandler(message));
                            break;
                        case "Message":
                            Task.Run(() => MessageHandler(message));
                            break;
                        case "Status":
                            break;
                        case "ClientStatus":
                            break;
                    }
                }
            }
        }

        catch (OperationCanceledException)
        {
            // Graceful exit
            Console.WriteLine("Message handling was canceled.");
        }
    }

    private void AddressHandler(string message)
    {
        var data = JsonSerializer.Deserialize<Address>(message);
        if (data == null) return;
        _addresses.Add(data.Url, data.ClientId);
        var ack = new Acknowledge(data.ClientId.ToString());
        _router.SendMoreFrame(data.ClientId.ToString());
        _router.SendFrame(JsonSerializer.Serialize(ack));
    }


    private void MessageHandler(string message)
    {
        var data = JsonSerializer.Deserialize<Message>(message);
        if (data == null) return;

        if (!_addresses.TryGetValue(data.Address, out var client)) return;
       
        var msg = JsonSerializer.Serialize(new Content(data.Content));
        _router.SendMoreFrame(client.ToString());
        _router.SendFrame(message);
    }

    public void Dispose()
    {
        _router.Dispose();
        _cancellation.Dispose();
    }
}