using System.Net.Sockets;
using System.Text;
using ChatServer.Messages;
using ChatServer.Rooms;

namespace ChatServer;

/// <summary>
/// Handles an individual client connection on the server.
/// </summary>
public class ClientHandler
{
    private TcpClient client;
    private Thread thread;
    private ChatServer server;
    private StreamWriter writer;
    private StreamReader reader;
    public string Name { get; set; } = "Anonymous";
    public Room Room { get; set; }

    public ClientHandler(TcpClient tcpClient, ChatServer server)
    {
        client = tcpClient;
        this.server = server;
        thread = new Thread(Run);
        thread.Start();
    }

    /// <summary>
    /// Main loop for handling client communication.
    /// Receives messages, handles commands (-nick, -join), and forwards messages to the dispatcher.
    /// </summary>
    private void Run()
    {
        try
        {
            using var stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8);

            Room = server.RoomManager.GetOrCreateRoom("general");
            Room.AddMember(this);

            server.AddClient(this);
            writer.AutoFlush = true;

            writer.WriteLine("Welcome to Chat Server!");
            writer.WriteLine("You are in room general.");
            writer.WriteLine("Use -nick <name> to set nickname.");
            writer.WriteLine("Use -join <name> to switch room.");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("-nick"))
                {
                    Name = line.Substring(6).Trim();
                    writer.WriteLine("Name set to: " + Name);
                }
                else if (line.StartsWith("-join"))
                {
                    var newRoom = line.Substring(6).Trim();
                    if (!string.IsNullOrEmpty(newRoom))
                    {
                        Room.RemoveMember(this);
                        Room = server.RoomManager.GetOrCreateRoom(newRoom);
                        Room.AddMember(this);
                        writer.WriteLine("Joined room: " + newRoom);
                    }
                }
                else
                {
                    server.Dispatcher.Enqueue(new Message(line, Room.Name,this));
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"Client {Name} disconnected");
            server.Logger.Log($"Client {Name} disconnected with error: {e.Message}");
        }
        finally
        {
            Room.RemoveMember(this);
            server.RemoveClient(this);
            client.Close();
        }
    }

    /// <summary>
    /// Sends a message to the client.
    /// </summary>
    /// <param name="message">Message text to send.</param>
    public void SendMessage(string message)
    {
        try
        {
            lock (writer)
            {
                writer.WriteLine(message);
            }
        }
        catch (ObjectDisposedException e)
        {
            server.RemoveClient(this);
        }
        catch (IOException e)
        {
            server.RemoveClient(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            server.Logger.Log($"Error in sending message: {e.Message}");
        }
    }
}