// See https://aka.ms/new-console-template for more information

using System.Configuration;
using System.Net;
using System.Net.Sockets;

var portStr = ConfigurationManager.AppSettings["ServerPort"];

int port = 5000;
if (!string.IsNullOrEmpty(portStr))
{
    int.TryParse(portStr, out port);
}

var ipStr = ConfigurationManager.AppSettings["ServerIP"];
IPAddress ip = IPAddress.Any;
if (!string.IsNullOrEmpty(ipStr))
{
    IPAddress.TryParse(ipStr, out ip);
}

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