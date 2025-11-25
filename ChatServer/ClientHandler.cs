using System.Net.Sockets;
using System.Text;

namespace ChatServer;

public class ClientHandler
{
    private TcpClient client;
    private Thread thread;

    public ClientHandler(TcpClient tcpClient)
    {
        client = tcpClient;
        thread = new Thread(Run);
        thread.IsBackground = true;
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
                writer.WriteLine(line);
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