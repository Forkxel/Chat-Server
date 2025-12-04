using ChatServer;
using ChatServer.Rooms;

namespace ChatServerTest;

public class MessageClientHandler : ClientHandler
{
    public List<string> Received { get; set; } = new();
    private string room = "general";

    public MessageClientHandler() : base(null, null)
    {
        this.Room = new Room(room);
    }

    public override void SendMessage(string message)
    {
        Received.Add(message);
    }
}