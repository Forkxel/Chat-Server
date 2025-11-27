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

var server = new ChatServer.ChatServer(ip,port);

try
{
    server.Start();
    Console.WriteLine("Type 'exit' to stop the server.");
    while ((Console.ReadLine() ?? "") != "exit") { }
}
catch (SocketException e) { }
catch (Exception e)
{
    Console.WriteLine("Server error: " + e.Message);
}
finally
{
    server.Stop();
}