using System.Net;
using System.Net.Sockets;
using ChatServer.Messages;
using ChatServer.Rooms;

namespace ChatServer;

/// <summary>
/// Main chat server class. Handles clients, rooms, and message dispatching.
/// </summary>
public class ChatServer
{
    private TcpListener listener;
    private Thread thread;
    public MessageDispatcher Dispatcher { get; set; }
    private bool running = true;
    private List<ClientHandler> clients = new();
    private object clientsLock = new();
    public RoomManager RoomManager { get; set; } = new();
    public Logger Logger { get; set; } = new();
    
    public ChatServer(IPAddress ip, int port)
    {
        listener = new TcpListener(ip, port);
        Dispatcher = new MessageDispatcher(() =>
        {
            lock (clientsLock) 
            {
                return clients.ToArray();
            }
        });
        thread = new Thread(AcceptClient);
    }

    /// <summary>
    /// Adds client to the List of clients
    /// </summary>
    /// <param name="clientHandler">Client to be added</param>
    public void AddClient(ClientHandler clientHandler)
    {
        lock (clientsLock)
        {
            clients.Add(clientHandler);
        }
    }

    /// <summary>
    /// Remove client from the list
    /// </summary>
    /// <param name="clientHandler">Client to be removed</param>
    public void RemoveClient(ClientHandler clientHandler)
    {
        lock (clientsLock)
        {
            clients.Remove(clientHandler);
            Logger.Log($"{clientHandler.Name} disconnected from the room {clientHandler.Room.Name}.");
        }
    }

    /// <summary>
    /// Creates a ClientHandler for each connected client.
    /// </summary>
    private void AcceptClient()
    {
        while (running)
        {
            try
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected " + client.Client.RemoteEndPoint);
                Logger.Log("Client connected " + client.Client.RemoteEndPoint);

                _ = new ClientHandler(client, this);
            }
            catch (SocketException e) when (!running) { }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Log("Error in accepting client: " + e.Message);
                throw;
            }
        }
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        listener.Start();
        thread.Start();
        Console.WriteLine("Server started");
    }

    /// <summary>
    /// Stops the server and message dispatcher.
    /// </summary>
    public void Stop()
    {
        try
        {
            running = false;
            listener.Stop();
            Dispatcher.Stop();
            if (thread != null && thread.IsAlive)
            {
                thread.Join();
            }

            Console.WriteLine("Server stopped");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Logger.Log("Error in stopping server: " + e.Message);
        }
        finally
        {
            Console.Write("Press any key to exit...");
            Console.ReadLine();
        }
    }
}