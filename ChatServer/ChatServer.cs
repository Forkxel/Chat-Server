using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class ChatServer
{
    private TcpListener listener;
    private Thread thread;
    private MessageDispatcher dispatcher = new();
    private bool running = true;

    public ChatServer(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        thread = new Thread(AcceptClient);
    }

    public void AcceptClient()
    {
        while (running)
        {
            try
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected " + client.Client.RemoteEndPoint);

                _ = new ClientHandler(client, dispatcher);
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
        dispatcher.Stop();
        thread.Join(500);
        Console.WriteLine("Server stopped");
    }
}