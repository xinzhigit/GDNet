// See https://aka.ms/new-console-template for more information

using Client;
using Net.Client;

Console.WriteLine("Hello, World!");

TcpClient client = new TcpClient();
client.Log += Console.WriteLine;

Example example = new Example();
client.AddRpcHandle(example);
client.Connect("127.0.0.1", 6666).Wait();
client.SendRT("example", "第一次进入服务器的OnUnClientRequest方法");
client.SendRT("example", "客户端Rpc请求");

while (true)
{
    Console.ReadLine();
}