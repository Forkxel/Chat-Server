using System.Net.Sockets;
using System.Text;

namespace ChatServer;

public class ClientHandler
{
    private TcpClient client;
    private Thread thread;
    private MessageDispatcher dispatcher;
    public string Name { get; set; }

    public ClientHandler(TcpClient tcpClient, MessageDispatcher dispatcher)
    {
        client = tcpClient;
        this.dispatcher = dispatcher;
        thread = new Thread(Run);
        thread.Start();
    }

    private void Run()
    {
        try
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8);

            writer.AutoFlush = true;

            writer.WriteLine("Type and it will echo");
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
                    dispatcher.Enqueue(new Message(line,this));
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
            client.Close();
        }
    }
}