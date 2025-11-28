// See https://aka.ms/new-console-template for more information

using System.Configuration;
using System.Net;
using System.Net.Sockets;

var portStr = ConfigurationManager.AppSettings["ServerPort"];
var ipStr = ConfigurationManager.AppSettings["ServerIP"];

if (string.IsNullOrEmpty(portStr) || !int.TryParse(portStr, out int port))
{
    Console.WriteLine("Invalid port number!");
    Console.WriteLine("Fix the config file.");
    Console.Write("Press any key to exit...");
    Console.ReadLine();
    return;
}

if (string.IsNullOrEmpty(ipStr) || !IPAddress.TryParse(ipStr, out IPAddress ip))
{
    Console.WriteLine("Invalid IP address!");
    Console.WriteLine("Fix the config file.");
    Console.Write("Press any key to exit...");
    Console.ReadLine();
    return;
}

Console.WriteLine($"Loaded configuration: IP: {ip} Port: {port}");

try
{
    var server = new ChatServer.ChatServer(ip, port);
    server.Start();
    Console.WriteLine("Type 'exit' to stop the server.");
    while ((Console.ReadLine() ?? "") != "exit")
    {
    }
    server.Stop();
}
catch (SocketException e)
{
    Console.WriteLine($"Cannot start server on {ip}:{port}. Check if the IP is available and the port is free. Fix App.config if necessary");
}
catch (Exception e)
{
    Console.WriteLine("Server error: " + e.Message);
}