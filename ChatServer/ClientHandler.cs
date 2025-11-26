using System.Net.Sockets;
using System.Text;

namespace ChatServer;

public class ClientHandler
{
    private TcpClient client;
    private Thread thread;
    private ChatServer server;
    private StreamWriter writer;
    private StreamReader reader;
    public string Name { get; set; }

    public ClientHandler(TcpClient tcpClient, ChatServer server)
    {
        client = tcpClient;
        this.server = server;
        thread = new Thread(Run);
        thread.Start();
    }

    private void Run()
    {
        try
        {
            using var stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8);

            server.AddClient(this);
            writer.AutoFlush = true;

            writer.WriteLine("Type message you want to send.");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("/nick"))
                {
                    Name = line.Substring(6).Trim();
                    writer.WriteLine("Name set to: " + Name);
                }
                else
                {
                    server.Dispatcher.Enqueue(new Message(line, this));
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            server.RemoveClient(this);
            client.Close();
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            lock (writer)
            {
                writer.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}