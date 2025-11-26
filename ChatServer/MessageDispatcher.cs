using System.Collections.Concurrent;

namespace ChatServer;

public class MessageDispatcher
{
    private BlockingCollection<Message> queue = new();
    private Thread thread;
    private bool running = true;

    public MessageDispatcher()
    {
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
                Console.WriteLine(message.Sender + ": " + message.Text);
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