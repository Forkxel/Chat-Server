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
            writer.WriteLine("Use /nick <name> to set nickname.");
            writer.WriteLine("Use /join <name> to switch room.");
            writer.WriteLine("Use /who to list users in room.");
            writer.WriteLine("Use /list to list all the rooms.");
            writer.WriteLine("Use /msg <nick> <message> to send private message.");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("-nick"))
                {
                    string newName = line.Substring(6).Trim();

                    if (string.IsNullOrEmpty(newName))
                    {
                        writer.WriteLine("Nickname cannot be empty.");
                        continue;
                    }

                    if (server.IsNicknameTaken(newName))
                    {
                        writer.WriteLine($"Nickname '{newName}' is already taken. Choose another one.");
                        continue;
                    }

                    Name = newName;
                    writer.WriteLine("Name set to: " + Name);
                }

                else if (line.StartsWith("/join"))
                {
                    var newRoom = line.Substring(6).Trim();
                    if (!string.IsNullOrEmpty(newRoom))
                    {
                        server.Dispatcher.Enqueue(new Message($"User {Name} left the room {Room.Name}", Room.Name, this));
                        Room.RemoveMember(this);
                        Room = server.RoomManager.GetOrCreateRoom(newRoom);
                        server.Dispatcher.Enqueue(new Message($"User {Name} joined the room {Room.Name}", Room.Name, this));
                        Room.AddMember(this);
                        writer.WriteLine("Joined room: " + newRoom);
                    }
                }
                else if (line.StartsWith("/who"))
                {
                    lock (Room)
                    {
                        var members = Room.GetMembers().Select(c => c.Name).ToList();
                        writer.WriteLine("Users in this room: " + string.Join(", ", members));
                    }
                }
                else if (line.StartsWith("/list"))
                {
                    var rooms = server.RoomManager.GetRoomNames();
                    writer.WriteLine("Available rooms: " + string.Join(", ", rooms));
                }
                else if (line.StartsWith("/msg"))
                {
                    var parts = line.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 3)
                    {
                        writer.WriteLine("Usage: /msg <nick> <message>");
                        continue;
                    }

                    var targetName = parts[1];
                    var messageText = parts[2];

                    var targetClient = server.GetClientByName(targetName);

                    if (targetClient == null)
                    {
                        writer.WriteLine($"User '{targetName}' not found.");
                        continue;
                    }

                    var privateMsg = $"[PM {DateTime.Now:HH:mm:ss}] {Name}: {messageText}";

                    targetClient.SendMessage(privateMsg);
                    
                    writer.WriteLine($"[PM to {targetName}] {messageText}");
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
    public virtual void SendMessage(string message)
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