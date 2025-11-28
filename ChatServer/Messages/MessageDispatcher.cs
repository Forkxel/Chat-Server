using System.Collections.Concurrent;

namespace ChatServer.Messages;

/// <summary>
/// Dispatches messages from clients to all other clients in the same room.
/// </summary>
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

    /// <summary>
    /// Enqueues a message to be sent to clients.
    /// </summary>
    public void Enqueue(Message message)
    {
        queue.Add(message);
    }

    /// <summary>
    /// Sends the messages to all clients in the Room
    /// </summary>
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
                
                var text = $"[{message.Time:HH:mm:ss}] [{message.Sender.Room.Name}] {message.Sender.Name}: {message.Text}";
                foreach (var client in clientsProvider().Where(c => c.Room == message.Sender.Room))
                {
                    try
                    {
                        client.SendMessage(text);
                        Logger.Log($"[{message.Sender.Room.Name}] {message.Sender.Name}: {message.Text}");
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

    /// <summary>
    /// Stops the dispatcher and waits for its thread to finish.
    /// </summary>
    public void Stop()
    {
        running = false;
        queue.CompleteAdding();
        thread.Join();
    }
}