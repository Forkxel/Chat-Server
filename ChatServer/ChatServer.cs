using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class ChatServer
{
    private TcpListener listener;
    private Thread thread;
    public MessageDispatcher Dispatcher { get; set; }
    private bool running = true;
    private List<ClientHandler> clients = new();
    private object clientsLock = new();
    

    public ChatServer(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        Dispatcher = new MessageDispatcher((() =>
        {
            lock (clientsLock) 
            {
                return clients.ToArray();
            }
        }));
        thread = new Thread(AcceptClient);
    }

    public void AddClient(ClientHandler clientHandler)
    {
        lock (clientsLock)
        {
            clients.Add(clientHandler);
        }
    }

    public void RemoveClient(ClientHandler clientHandler)
    {
        lock (clientsLock)
        {
            clients.Remove(clientHandler);
        }
    }

    public void AcceptClient()
    {
        while (running)
        {
            try
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected " + client.Client.RemoteEndPoint);

                _ = new ClientHandler(client, this);
            }
            catch (SocketException e) when (!running) { }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Start()
    {
        listener.Start();
        thread.Start();
        Console.WriteLine("Server started");
    }

    public void Stop()
    {
        running = false;
        listener.Stop();
        thread.Join(500);
        Console.WriteLine("Server stopped");
    }
}