using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Helper
{
    /// <summary>
    /// 远程过程调用(RPC)帮助类
    /// </summary>
    public class RpcHelper
    {
        public static void AddRpc(IRpcHandler handle, object target, bool append, Action<SyncVarInfo> onSyncVarCollect)
        {
            AddRpc(handle, target, target.GetType(), append, onSyncVarCollect);
        }

        public static void AddRpc(IRpcHandler handle, object target, Type type, bool append, Action<SyncVarInfo> onSyncVarCollect)
        {
            AddRpc(handle, target, type, append, onSyncVarCollect, null, (member) => new RPCMethod(target, member.member as MethodInfo, member.rpc.cmd));
        }

        public static void AddRpc(IRpcHandler handle, object target, Type type, bool append, Action<SyncVarInfo> onSyncVarCollect, Action<MemberInfo, MemberData> action, Func<MemberData, IRPCMethod> func)
        {
            if (!append)
                if (handle.RpcTargetHash.ContainsKey(target))
                    return;
            if (!handle.MemberInfos.TryGetValue(type, out var list))
            {
                list = new List<MemberData>();
                var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var info in members)
                {
                    var data = new MemberData() { member = info };
                    if (info.MemberType == MemberTypes.Method)
                    {
                        var attributes = info.GetCustomAttributes(typeof(RPCFun), true);//兼容ILR写法
                        if (attributes.Length > 0)
                        {
                            data.rpc = attributes[0] as RPCFun;
                            action?.Invoke(info, data);
                        }
                    }
                    else if (info.MemberType == MemberTypes.Field | info.MemberType == MemberTypes.Property)
                    {
                        SyncVarHelper.InitSyncVar(info, target, (syncVar) =>
                        {
                            data.syncVar = syncVar;
                        });
                    }
                    if (data.rpc != null | data.syncVar != null)
                        list.Add(data);
                }
                handle.MemberInfos.Add(type, list);
            }
            foreach (var member in list)
            {
                var rpc = member.rpc;
                if (rpc != null)
                {
                    var item = func(member);
                    if (rpc.hash != 0)
                    {
                        if (!handle.RpcHashDic.TryGetValue(rpc.hash, out var dict))
                            handle.RpcHashDic.Add(rpc.hash, dict = new MyDictionary<object, IRPCMethod>());
                        dict.Add(target, item);
                    }
                    if (!handle.RpcDic.TryGetValue(item.method.Name, out var dict1))
                        handle.RpcDic.Add(item.method.Name, dict1 = new MyDictionary<object, IRPCMethod>());
                    dict1.Add(target, item);
                }
                var syncVar = member.syncVar;
                if (syncVar != null)
                {
                    var syncVar1 = syncVar.Clone(target);
                    if (syncVar1.id == 0)
                        onSyncVarCollect?.Invoke(syncVar1);
                    else
                        handle.SyncVarDic.TryAdd(syncVar1.id, syncVar1);
                }
            }
            if (list.Count > 0)
                handle.RpcTargetHash.Add(target, new MemberDataList() { members = list });
        }

        public static void RemoveRpc(IRpcHandler handle, object target)
        {
            if (handle.RpcTargetHash.TryRemove(target, out var list))
            {
                foreach (var item in list.members)
                {
                    if (item.rpc != null)
                    {
                        if (handle.RpcHashDic.TryGetValue(item.rpc.hash, out var dict))
                        {
                            dict.Remove(target);
                        }
                        if (handle.RpcDic.TryGetValue(item.member.Name, out dict))
                        {
                            dict.Remove(target);
                        }
                    }
                    if (item.syncVar != null)
                    {
                        handle.SyncVarDic.Remove(item.syncVar.id);
                    }
                }
            }
        }

        public static void Invoke(IRpcHandler handle, RPCModel model, Action<MyDictionary<object, IRPCMethod>> action, Action<int> log)
        {
            MyDictionary<object, IRPCMethod> methods;
            if (model.methodHash != 0)
            {
                if (handle.RpcTasks1.TryGetValue(model.methodHash, out var model1))
                {
                    model1.model = model;
                    model1.IsCompleted = true;
                    model1.referenceCount--;
                    if (model1.referenceCount <= 0)
                        handle.RpcTasks1.TryRemove(model.methodHash, out _);
                    if (model1.intercept)
                        return;
                }
                if (!handle.RpcHashDic.TryGetValue(model.methodHash, out methods))
                {
                    log(0);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.func))
                    return;
                if (handle.RpcTasks.TryGetValue(model.func, out var model1))
                {
                    model1.model = model;
                    model1.IsCompleted = true;
                    model1.referenceCount--;
                    if (model1.referenceCount <= 0)
                        handle.RpcTasks.TryRemove(model.func, out _);
                    if (model1.intercept)
                        return;
                }
                if (!handle.RpcDic.TryGetValue(model.func, out methods))
                {
                    log(1);
                    return;
                }
            }
            if (methods.Count <= 0)
            {
                log(2);
                return;
            }
            action(methods);
        }

        //public static void CheckRpc(IRpcHandler handle)//unity 2020版本以上 检测失效, 请在OnDestry中移除rpc
        //{
        //    //foreach (var item in handle.RpcTargetHash)
        //    //{
        //    //    if (ReferenceEquals(item.Key, null))
        //    //    {
        //    //        foreach (var item1 in item.Value.members)
        //    //        {
        //    //            if (item1.rpc != null)
        //    //            {
        //    //                if (handle.RpcHashDic.TryGetValue(item1.rpc.hash, out var dict))
        //    //                    dict.Remove(item.Key);
        //    //                if (handle.RpcDic.TryGetValue(item1.member.Name, out dict))
        //    //                    dict.Remove(item.Key);
        //    //            }
        //    //            if (item1.syncVar != null)
        //    //                handle.SyncVarDic.Remove(item1.syncVar.id);
        //    //        }
        //    //        handle.RpcTargetHash.Remove(item.Key);
        //    //    }
        //    //}
        //}
    }
}