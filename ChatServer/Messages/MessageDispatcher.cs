using System.Collections.Concurrent;
using System.Net.Mime;

namespace ChatServer;

public class MessageDispatcher
{
    private BlockingCollection<Message> queue = new();
    private Func<ClientHandler[]> clientsProvider;
    private Thread thread;
    private bool running = true;

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
                var message = queue.Take();
                var text = $"[{message.Time:HH:mm:ss}] {message.Sender}: {message.Text}";
                foreach (var client in clientsProvider().Where(c => c.Room == message.Sender.Room))
                {
                    try
                    {
                        client.SendMessage(text);
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
        thread.Join(500);
    }
}