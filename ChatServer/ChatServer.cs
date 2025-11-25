using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class ChatServer
{
    private TcpListener Listener;
    private Thread thread;
    private bool running = true;

    public ChatServer(int port)
    {
        Listener = new TcpListener(IPAddress.Any, port);
        thread = new Thread(AcceptClient);
    }

    public void AcceptClient()
    {
        while (running)
        {
            try
            {
                var client = Listener.AcceptTcpClient();
                Console.WriteLine("Client connected " + client.Client.RemoteEndPoint);
                
                client.Close();
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
        Listener.Start();
        thread.Start();
        Console.WriteLine("Server started");
    }

    public void Stop()
    {
        running = false;
        Listener.Stop();
        thread.Join(500);
        Console.WriteLine("Server stopped");
    }
}