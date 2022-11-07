// See https://aka.ms/new-console-template for more information

using Server;

Console.WriteLine("Hello, World!");

var server = new Service();
server.Log += Console.WriteLine;

server.Run(6666);
while (true)
{
    Console.ReadLine();
}