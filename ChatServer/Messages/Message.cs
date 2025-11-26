namespace ChatServer;

public class Message
{
    public string Text { get; set; }
    public ClientHandler Sender { get; set; }
    public DateTime Time { get; set; }

    public Message(string text, ClientHandler sender)
    {
        Text = text;
        Sender = sender;
        Time = DateTime.Now;
    }
}