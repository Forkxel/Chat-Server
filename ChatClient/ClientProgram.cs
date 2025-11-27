using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient;

/// <summary>
/// Main class for the chat client that connects to a chat server.
/// </summary>
public class ClientProgram
{
    /// <summary>
    /// Starts the client, prompts the user for server IP, and allows sending messages.
    /// </summary>
    public void Run()
    {
        string ip;
        int port;
        while (true)
        {
            Console.Write("Server IP: ");
            ip = Console.ReadLine();

            if (IPAddress.TryParse(ip, out _))
            {
                break;
            }
            Console.WriteLine("Invalid IP address. Please try again.");
            Console.Write("Port: ");
        }

        while (true)
        {
            Console.Write("Server Port: ");
            if (int.TryParse(Console.ReadLine(), out port))
            {
                break;
            }
            Console.WriteLine("Invalid port. Please try again.");
        }

        try
        {
            var client = new TcpClient(ip, port);
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
                if (!string.IsNullOrEmpty(input))
                {
                    writer.WriteLine(input);
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("Could not connect to the server.");
        }
        catch (IOException e)
        {
            Console.WriteLine("Connection lost.");
        }
        finally
        {
            Console.Write("\nPress any key to exit...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Reads messages from the server and prints them to the console.
    /// </summary>
    /// <param name="reader">StreamReader to read messages from the server.</param>
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
