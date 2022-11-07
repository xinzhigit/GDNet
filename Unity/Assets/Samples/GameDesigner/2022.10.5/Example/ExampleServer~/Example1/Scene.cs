using Net.Component;
using Net.Server;
using Net.Share;

namespace Example1
{
    internal class Scene : NetScene<Client>
    {
        public Scene() 
        {
            //SendOperationReliable = true;
        }

        public override void OnExit(Client client)
        {
            if (CurrNum <= 0) //如果没人时要清除操作数据，不然下次进来会直接发送Command.OnPlayerExit指令给客户端，导致客户端的对象被销毁
                operations.Clear();
            else
                AddOperation(new Operation(Command.OnPlayerExit, client.UserID));
        }
    }
}