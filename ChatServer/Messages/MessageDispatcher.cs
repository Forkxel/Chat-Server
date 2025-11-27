using System.Collections.Concurrent;

namespace ChatServer.Messages;

public class MessageDispatcher
{
    private BlockingCollection<Message> queue = new();
    private Func<ClientHandler[]> clientsProvider;
    private Thread thread;
    private bool running = true;
    public Logger Logger { get; } = new();

    public MessageDispatcher(Func<ClientHandler[]> clientsProvider)
    {
        this.clientsProvider = clientsProvider;
        thread = new Thread(Send);
        thread.Start();
    }

    public void Enqueue(Message message)
    {
        queue.Add(message);
    }

    private void Send()
    {
        while (running || queue.Count > 0)
        {
            try
            {
                Message message;
                try
                {
                    message = queue.Take();
                }
                catch (InvalidOperationException e)
                {
                    break;
                }
                
                var text = $"[{message.Time:HH:mm:ss}] {message.Sender.Name}: {message.Text}";
                foreach (var client in clientsProvider().Where(c => c.Room == message.Sender.Room))
                {
                    try
                    {
                        client.SendMessage(text);
                        Logger.Log($"[{message.Sender.Room}] {message.Sender.Room.Name}: {message.Text}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Stop()
    {
        running = false;
        queue.CompleteAdding();
        thread.Join();
    }
}