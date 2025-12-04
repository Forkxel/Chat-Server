// See https://aka.ms/new-console-template for more information

using System.Configuration;
using System.Net;
using System.Net.Sockets;

int port = 0;
while (true)
{
    var portStr = ConfigurationManager.AppSettings["ServerPort"];
    if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out port))
        break;

    Console.WriteLine("Invalid port number in App.config! Fix the config file.");
    Console.Write("Enter a new port: ");
    portStr = Console.ReadLine();
    if (!int.TryParse(portStr, out port))
    {
        Console.WriteLine("Port must be a number!");
        continue;
    }
}


IPAddress ip = null;
while (true)
{
    var ipStr = ConfigurationManager.AppSettings["ServerIP"];
    if (!string.IsNullOrEmpty(ipStr) && IPAddress.TryParse(ipStr, out ip))
        break;

    Console.WriteLine("Invalid IP address in App.config! Fix the config file.");
    Console.Write("Enter a new IP: ");
    ipStr = Console.ReadLine();
    if (!IPAddress.TryParse(ipStr, out ip))
    {
        Console.WriteLine("IP address format is invalid!");
    }
}


Console.WriteLine($"Loaded configuration: IP: {ip} Port: {port}");

while (true)
{
    try
    {
        var server = new ChatServer.ChatServer(ip, port);
        server.Start();
        Console.WriteLine("Type 'exit' to stop the server.");
        while ((Console.ReadLine() ?? "").ToLower() != "exit") { }
        server.Stop();
        break;
    }
    catch (SocketException)
    {
        Console.WriteLine($"Cannot start server on {ip}:{port}. Check if the IP is available and the port is free.");
        Console.Write("Enter a new IP: ");
        while (!IPAddress.TryParse(Console.ReadLine(), out ip))
        {
            Console.WriteLine("IP invalid. Try again.");
            Console.Write("Enter a new IP: ");
        }

        Console.Write("Enter a new Port: ");
        while (!int.TryParse(Console.ReadLine(), out port))
        {
            Console.WriteLine("Port invalid. Try again.");
            Console.Write("Enter a new Port: ");
        }
    }
}