using ChatServer.Rooms;

namespace ChatServer;

public class Message
{
    public string Text { get; set; }
    public string Room { get; set; }
    public ClientHandler Sender { get; set; }
    public DateTime Time { get; set; }

    public Message(string text, string room, ClientHandler sender)
    {
        Text = text;
        Room = room;
        Sender = sender;
        Time = DateTime.Now;
    }
}