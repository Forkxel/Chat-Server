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
    private static object historyLocker = new();

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
            writer.WriteLine("Use /delete <roomName> to delete room");
            writer.WriteLine("Use /clear to delete history of current room.");
            writer.WriteLine();
            
            SendRoomHistory(Room);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("/nick"))
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
                        var oldRoom = Room;
                        oldRoom.RemoveMember(this);
                        foreach (var member in oldRoom.GetMembers())
                        {
                            member.SendMessage($"User {Name} left the room {oldRoom.Name}");
                        }
                        
                        Room = server.RoomManager.GetOrCreateRoom(newRoom);
                        Room.AddMember(this);
                        foreach (var member in Room.GetMembers().Where(m => m != this))
                        {
                            member.SendMessage($"User {Name} joined the room {Room.Name}");
                        }
                        
                        writer.WriteLine($"Joined room: {Room.Name}");
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
                    
                    if (targetName.Equals("Anonymous", StringComparison.OrdinalIgnoreCase))
                    {
                        writer.WriteLine("You cannot send private messages to Anonymous.");
                        return;
                    }

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
                else if (line.StartsWith("/delete"))
                {
                    var roomToRemove = line.Substring(8).Trim();
                    if (!string.IsNullOrEmpty(roomToRemove))
                    {
                        if (server.RoomManager.RemoveRoom(roomToRemove))
                        {
                            writer.WriteLine($"Room '{roomToRemove}' removed and history deleted.");
                        }
                        else
                        {
                            writer.WriteLine($"Room '{roomToRemove}' does not exist.");
                        }
                    }
                }
                else if (line.StartsWith("/clear"))
                {
                    Room.ClearHistory();
                }
                else
                {
                    var msg = new Message(line, Room.Name, this);
                    server.Dispatcher.Enqueue(msg);
                    AppendToRoomHistory(Room, msg);
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"Client {Name} disconnected");
            Logger.Log($"Client {Name} disconnected with error: {e.Message}");
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
            Logger.Log($"Error in sending message: {e.Message}");
        }
    }
    
    private void SendRoomHistory(Room room)
    {
        var file = $"{room.Name}_history.txt";
        if (File.Exists(file))
        {
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                lock (writer)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }

    private void AppendToRoomHistory(Room room, Message message)
    {
        var file = $"{room.Name}_history.txt";
        var line = $"[{message.Time:HH:mm:ss}] [{room.Name}] {message.Sender.Name}: {message.Text}";
        
        lock (historyLocker)
        {
            File.AppendAllText(file, line + Environment.NewLine);
        }
        
        Logger.Log(line);
    }
}