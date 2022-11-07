using Net.System;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Net.Share
{
    public interface IRpcHandler
    {
        /// <summary>
        /// 远程方法优化字典
        /// </summary>
        MyDictionary<string, MyDictionary<object, IRPCMethod>> RpcDic { get; set; }
        /// <summary>
        /// 远程方法哈希字典
        /// </summary>
        MyDictionary<ushort, MyDictionary<object, IRPCMethod>> RpcHashDic { get; set; }
        /// <summary>
        /// 已经收集过的类信息
        /// </summary>
        MyDictionary<Type, List<MemberData>> MemberInfos { get; set; }
        /// <summary>
        /// 当前收集rpc的对象信息
        /// </summary>
        MyDictionary<object, MemberDataList> RpcTargetHash { get; set; }
        /// <summary>
        /// 字段同步信息
        /// </summary>
        MyDictionary<ushort, SyncVarInfo> SyncVarDic { get; set; }
        /// <summary>
        /// 等待回调的异步Rpc
        /// </summary>
        ConcurrentDictionary<string, RPCModelTask> RpcTasks { get; set; }
        /// <summary>
        /// 等待回调的异步Rpc
        /// </summary>
        ConcurrentDictionary<ushort, RPCModelTask> RpcTasks1 { get; set; }
        /// <summary>
        /// Rpc任务队列
        /// </summary>
        QueueSafe<IRPCData> RpcWorkQueue { get; set; }
        /// <summary>
        /// 移除target的所有rpc
        /// </summary>
        /// <param name="target"></param>
        void RemoveRpc(object target);
        /// <summary>
        /// 检查rpc对象或方法是否已被销毁（释放）
        /// </summary>
        //void CheckRpc();
    }
}