using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient;

public class ClientProgram
{
    public void Run()
    {
        string ip;
        while (true)
        {
            Console.Write("Server IP: ");
            ip = Console.ReadLine();

            if (IPAddress.TryParse(ip, out _))
            {
                break;
            }
            Console.WriteLine("Invalid IP address. Please try again.");
        }

        try
        {
            var client = new TcpClient(ip, 5000);
            var stream = client.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            var writer = new StreamWriter(stream, Encoding.UTF8);
        
            writer.AutoFlush = true;

            Console.Clear();
            Console.WriteLine("Connection established.\n");
        
            new Thread(() => WriteToConsole(reader)).Start();
        
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                writer.WriteLine(input);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("Could not connect to the server.");
        }
    }

    private void WriteToConsole(StreamReader reader)
    {
        try
        {
            while (true)
            {
                string msg = reader.ReadLine();
                if (!string.IsNullOrEmpty(msg))
                {
                    Console.WriteLine(msg);
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("Disconnected.");
        }
    }
}
