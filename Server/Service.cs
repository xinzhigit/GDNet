using Net.Server;
using Net.Share;

namespace Server;

public class Client : NetPlayer
{
    
}

public class Scene : NetScene<Client>
{
    
}

public class Service : TcpServer<Client, Scene>
{
    protected override bool OnUnClientRequest(Client unClient, RPCModel model)
    {
        Console.WriteLine(model.pars[0]);
        
        return true;
    }

    [Rpc(cmd = NetCmd.SafeCall)]
    void Test(Client client, string str)
    {
        Console.WriteLine(str);
        SendRT(client, "test", "服务器Rpc回调");
    }
}