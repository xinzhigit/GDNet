namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// tcp 输入输出完成端口服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class TcpServerIocp<Player, Scene> : TcpServer<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        protected override void StartSocketHandler()
        {
            AcceptHandler();
        }

        protected override void CreateServerSocket(ushort port)
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = new IPEndPoint(IPAddress.Any, port);
            Server.NoDelay = true;
            Server.Bind(ip);
            Server.Listen(LineUp);
        }

        private void AcceptHandler()
        {
            try
            {
                if (!IsRunServer)
                    return;
                if (ServerArgs == null)
                {
                    ServerArgs = new SocketAsyncEventArgs();
                    ServerArgs.Completed += OnIOCompleted;
                }
                ServerArgs.AcceptSocket = null;// 重用前进行对象清理
                if (!Server.AcceptAsync(ServerArgs))
                    OnIOCompleted(null, ServerArgs);
            }
            catch (Exception ex)
            {
                Debug.Log($"接受异常:{ex}");
            }
        }

        protected override void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            Socket clientSocket = null;
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    try
                    {
                        clientSocket = args.AcceptSocket;
                        if (clientSocket.RemoteEndPoint == null)
                            return;
                        var client = AcceptHander(clientSocket, clientSocket.RemoteEndPoint);
                        client.ReceiveArgs = new SocketAsyncEventArgs();
                        client.ReceiveArgs.UserToken = client;
                        client.ReceiveArgs.RemoteEndPoint = clientSocket.RemoteEndPoint;
                        client.ReceiveArgs.SetBuffer(new byte[ushort.MaxValue], 0, ushort.MaxValue);
                        client.ReceiveArgs.Completed += OnIOCompleted;
                        if (!clientSocket.ReceiveAsync(client.ReceiveArgs))
                            OnIOCompleted(null, client.ReceiveArgs);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        AcceptHandler();
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    var client1 = args.UserToken as Player;
                    int count = args.BytesTransferred;
                    if (count > 0 & args.SocketError == SocketError.Success)
                    {
                        var buffer = BufferPool.Take();
                        buffer.Count = count;
                        Buffer.BlockCopy(args.Buffer, args.Offset, buffer, 0, count);
                        receiveCount += count;
                        receiveAmount++;
                        if (client1.isDispose)
                        {
                            BufferPool.Push(buffer);
                            return;
                        }
                        ResolveBuffer(client1, ref buffer);
                        BufferPool.Push(buffer);
                        clientSocket = client1.Client;
                        if (!clientSocket.Connected)
                            return;
                        if (!clientSocket.ReceiveAsync(args))
                            OnIOCompleted(null, args);
                    }
                    break;
            }
        }

        //protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        //{
        //    if (!client.Client.Connected)
        //        return;
        //    if (buffer.Length <= frame)//解决长度==6的问题(没有数据)
        //        return;
        //    if (client.Client.Poll(1, SelectMode.SelectWrite))
        //    {
        //        using (var args = new SocketAsyncEventArgs())
        //        {
        //            args.SetBuffer(buffer, 0, buffer.Length);
        //            args.RemoteEndPoint = client.RemotePoint;
        //            args.Completed += OnIOCompleted;
        //            if (!client.Client.SendAsync(args))
        //                OnIOCompleted(client, args);
        //        }
        //        sendAmount++;
        //        sendCount += buffer.Length;
        //    }
        //    else
        //    {
        //        Debug.LogError($"[{client}]发送缓冲列表已经超出限制!");
        //    }
        //}
    }

    /// <summary>
    /// 默认tcpiocp服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class TcpServerIocp : TcpServerIocp<NetPlayer, DefaultScene> { }
}