using Net.Event;
using Net.Share;
using Net.System;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Net.Server
{
    /// <summary>
    /// udp 输入输出完成端口服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class UdpServerIocp<Player, Scene> : ServerBase<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        protected override void StartSocketHandler()
        {
            ServerArgs = new SocketAsyncEventArgs { UserToken = Server };
            ServerArgs.Completed += OnIOCompleted;
            ServerArgs.SetBuffer(new byte[ushort.MaxValue], 0, ushort.MaxValue);
            ServerArgs.RemoteEndPoint = Server.LocalEndPoint;
            if (!Server.ReceiveFromAsync(ServerArgs))
                OnIOCompleted(null, ServerArgs);
        }

        protected override void AcceptHander(Player client)
        {
            client.Gcp = new Plugins.GcpKernel();
            client.Gcp.MTU = (ushort)MTU;
            client.Gcp.RTO = RTO;
            client.Gcp.MTPS = MTPS;
            client.Gcp.FlowControl = FlowControl;
            client.Gcp.RemotePoint = client.RemotePoint;
            client.Gcp.OnSender += (bytes) => {
                Send(client, NetCmd.ReliableTransport, bytes);
            };
        }

        protected unsafe override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            if (buffer.Length >= 65507)
            {
                NDebug.LogError($"[{client}] 数据太大! 请使用SendRT");
                return;
            }
            var args = ObjectPool<SocketAsyncEventArgs>.Take(args1 =>
            {
                args1.Completed += OnIOCompleted;
                args1.SetBuffer(new byte[ushort.MaxValue], 0, ushort.MaxValue);
            });
            args.RemoteEndPoint = client.RemotePoint;
            var buffer1 = args.Buffer;
            Buffer.BlockCopy(buffer, 0, buffer1, 0, buffer.Length);
            args.SetBuffer(0, buffer.Length);
            if (!Server.SendToAsync(args))
                OnIOCompleted(client, args);
            sendAmount++;
            sendCount += buffer.Length;
        }
    }

    /// <summary>
    /// 默认udpiocp服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class UdpServerIocp : UdpServerIocp<NetPlayer, DefaultScene> { }
}
