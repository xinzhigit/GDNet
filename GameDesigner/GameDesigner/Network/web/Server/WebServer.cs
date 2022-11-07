namespace Net.Server
{
    using Fleck;
    using Net.Serialize;
    using Net.Share;
    using Newtonsoft_X.Json;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Text;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// web网络服务器 2020.8.25 七夕
    /// 通过JavaScript脚本, httml网页进行连接. 和 WebClient连接
    /// 客户端发送的数据请求请看Net.Share.MessageModel类定义
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    [Obsolete("尚未实测!遇到问题群里说下，给你解决!", false)]
    public class WebServer<Player, Scene> : ServerBase<Player, Scene> where Player : WebPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// webSocket服务器套接字
        /// </summary>
        public new WebSocketServer Server { get; protected set; }

        protected override void CreateServerSocket(ushort port)
        {
            var ipAddress = IPAddress.Any;
            var ip = new IPEndPoint(ipAddress, port);
            Server = new WebSocketServer($"ws://{ip}");
            Server.ListenerSocket.NoDelay = true;
            Server.Start(AcceptConnect);
        }

        protected override void ReceiveProcessed(EndPoint remotePoint, ref bool isSleep)
        {
        }

        //开始接受客户端连接
        private void AcceptConnect(IWebSocketConnection wsClient)
        {
            var wsClient1 = wsClient as WebSocketConnection;
            var clientSocket = ((SocketWrapper)wsClient1.Socket)._socket;
            var remotePoint = clientSocket.RemoteEndPoint;
            wsClient1.OnOpen = () =>
            {
                if (!UserIDStack.TryPop(out int uid))
                    uid = GetCurrUserID();
                var client = AcceptHander(clientSocket, remotePoint);
                client.WSClient = wsClient1;
            };
            wsClient1.OnMessage = (buffer, message) => //utf-8解析
            {
                receiveCount += buffer.Length;
                receiveAmount++;
                if (AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
                    WSRevdHandle(client, buffer, message);
            };
            wsClient1.OnBinary = (buffer) =>
            {
                var segment = BufferPool.Take(buffer.Length);
                Buffer.BlockCopy(buffer, 0, segment, 0, buffer.Length);
                segment.Count = buffer.Length;
                receiveCount += buffer.Length;
                receiveAmount++;
                if (AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
                {
                    client.RevdQueue.Enqueue(segment);
                }
            };
            wsClient1.OnClose = () =>
            {
                if (AllClients.TryGetValue(remotePoint, out Player player))
                    RemoveClient(player);
            };
        }

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect)
                return true;
            if (model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        private void WSRevdHandle(Player client, byte[] buffer, string message)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<MessageModel>(message);
                var model1 = new RPCModel(model.cmd, model.func, model.GetPars()) {
                    buffer = buffer, count = buffer.Length
                };
                DataHandle(client, model1, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{client}]json解析出错:" + ex);
                var model = new MessageModel(0, "error", new object[] { ex.Message });
                string jsonStr = JsonConvert.SerializeObject(model);
                client.WSClient.Send(jsonStr);
            }
        }

        protected override void CheckHeart(Player client)
        {
            if (!client.Client.Connected)
            {
                RemoveClient(client);
                return;
            }
            if (client.heart > HeartLimit * 5)
            {
                client.Redundant = true;
                RemoveClient(client);
                return;
            }
            client.heart++;
            if (client.heart <= HeartLimit)//确认心跳包
                return;
            SendRT(client, NetCmd.SendHeartbeat, new byte[0]);//保活连接状态
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            client.WSClient.Send(buffer);
            sendAmount++;
            sendCount += buffer.Length;
        }

        protected override void SceneUpdateHandle()
        {
        }

        protected override byte[] OnSerializeRpc(RPCModel model)
        {
            if (!string.IsNullOrEmpty(model.func) | model.methodHash != 0)
            {
                var model1 = new MessageModel(model.cmd, model.func, model.pars);
                string jsonStr = JsonConvert.SerializeObject(model1);
                byte[] jsonStrBytes = Encoding.UTF8.GetBytes(jsonStr);
                byte[] bytes = new byte[jsonStrBytes.Length + 1];
                bytes[0] = 32; //32=utf8的" "空字符
                Buffer.BlockCopy(jsonStrBytes, 0, bytes, 1, jsonStrBytes.Length);
                return bytes;
            }
            return NetConvert.Serialize(model, new byte[] { 10 });//10=utf8的\n字符
        }

        protected override FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (buffer[index++] == 32)
            {
                var message = Encoding.UTF8.GetString(buffer, index, count - 1);
                var model = JsonConvert.DeserializeObject<MessageModel>(message);
                return new FuncData(model.func, model.GetPars());
            }
            return NetConvert.Deserialize(buffer, index, count - 1);
        }
    }

    /// <summary>
    /// 默认web服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class WebServer : WebServer<WebPlayer, NetScene<WebPlayer>>
    {
    }
}