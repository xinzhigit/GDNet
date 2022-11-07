using Net.Share;
using System;
using System.Reflection;
using Net.Server;
using System.Threading;
using Net.Event;
using Net.System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Net.Helper;
using Net.Client;
using System.Collections.Concurrent;

namespace Net.Adapter
{
    public class RPCPTR
    {
        public byte cmd;
        public MethodInfo method;
        public virtual void Invoke(object target, object[] pars) {}
    }
    public class RPCPTRMethod : RPCPTR
    {
        public override void Invoke(object target, object[] pars)
        {
            method.Invoke(target, pars);
        }
    }
    public class RPCPTRNull : RPCPTR
    {
        public Action ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr();
        }
    }
    public class RPCPTR<T> : RPCPTR
    {
        public Action<T> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0]);
        }
    }
    public class RPCPTR<T, T1> : RPCPTR
    {
        public Action<T, T1> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1]);
        }
    }
    public class RPCPTR<T, T1, T2> : RPCPTR
    {
        public Action<T, T1, T2> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2]);
        }
    }
    public class RPCPTR<T, T1, T2, T3> : RPCPTR
    {
        public Action<T, T1, T2, T3> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8], (T9)pars[9]);
        }
    }
    public class RPCPTR<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : RPCPTR
    {
        public Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ptr;
        public override void Invoke(object target, object[] pars)
        {
            ptr((T)pars[0], (T1)pars[1], (T2)pars[2], (T3)pars[3], (T4)pars[4], (T5)pars[5], (T6)pars[6], (T7)pars[7], (T8)pars[8], (T9)pars[9], (T10)pars[10]);
        }
    }

    /// <summary>
    /// 服务器远程过程调用适配器
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    public class CallSiteRpcAdapter<Player> : CallSiteRpcAdapter, IRPCAdapter<Player> where Player : NetPlayer
    {
        /// <summary>
        /// 构造客户端Rpc适配器，参数是xxxServer对象
        /// </summary>
        /// <param name="handle"></param>
        public CallSiteRpcAdapter(IRpcHandler handle) : base(handle)
        {
        }

        public void OnRpcExecute(Player client, RPCModel model)
        {
            RpcHelper.Invoke(handle, model, methods =>
            {
                foreach (RPCMethodPtr rpc in methods.Values)
                {
                    try
                    {
                        if (rpc.cmd == NetCmd.SafeCall)
                        {
                            object[] pars = new object[model.pars.Length + 1];
                            pars[0] = client;
                            Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                            rpc.Invoke(pars);
                        }
                        else if (rpc.cmd == NetCmd.SingleCall)
                        {
                            ThreadManager.Invoke(() =>
                            {
                                object[] pars = new object[model.pars.Length + 1];
                                pars[0] = client;
                                Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                                rpc.Invoke(pars);
                            });
                        }
                        else
                        {
                            rpc.Invoke(model.pars);
                        }
                    }
                    catch (Exception ex)
                    {
                        NDebug.LogError($"方法:{rpc.method} {model} 详细信息:{ex}");
                    }
                }
            }, log => {
                switch (log)
                {
                    case 0:
                        NDebug.LogWarning($"{client} [mask:{model.methodHash}]的远程方法未被收集!请定义[Rpc(hash = {model.methodHash})] void xx方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                    case 1:
                        NDebug.LogWarning($"{client} {model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                    case 2:
                        NDebug.LogWarning($"{client} {model}的远程方法未被收集!请定义[Rpc]void xx方法和参数, 并使用server.AddRpc方法收集rpc方法!");
                        break;
                }
            });
        }
    }

    /// <summary>
    /// 客户端远程过程调用适配器
    /// </summary>
    public class CallSiteRpcAdapter : IRPCAdapter
    {
        protected IRpcHandler handle;

#if UNITY_EDITOR
        private readonly bool useIL2CPP;
#endif
        /// <summary>
        /// 构造客户端Rpc适配器，参数是xxxClient对象
        /// </summary>
        /// <param name="handle"></param>
        public CallSiteRpcAdapter(IRpcHandler handle)
        {
            this.handle = handle;
#if UNITY_EDITOR
#pragma warning disable CS0618 // 类型或成员已过时
            useIL2CPP = UnityEditor.PlayerSettings.GetPropertyInt("ScriptingBackend", UnityEditor.BuildTargetGroup.Standalone) == 1;
#pragma warning restore CS0618 // 类型或成员已过时
#endif
        }

        public void AddRpcHandle(object target, bool append, Action<SyncVarInfo> onSyncVarCollect)
        {
            var type = target.GetType();
            RpcHelper.AddRpc(handle, target, type, append, onSyncVarCollect, (info, data) =>
            {
                var method = info as MethodInfo;
                if (method.ReturnType != typeof(void))
                {
                    var met1 = new RPCPTRMethod
                    {
                        method = method,
                        cmd = data.rpc.cmd
                    };
                    data.ptr = met1;
                    return;
                }
                var pars = method.GetParameters();
#if UNITY_EDITOR
                if (data.rpc.il2cpp == null & useIL2CPP)
                    throw new Exception("如果在unity编译为il2cpp后端脚本，则需要先声明类型出来，因为编译后，类型被固定，将无法创建出来! 例子: void Test(int num, string str); 则需要这样添加 [Rpc(il2cpp = typeof(RPCPTR<int, string>))]");
                if (useIL2CPP) 
                {
                    var pars1 = data.rpc.il2cpp.GetGenericArguments();
                    if (pars.Length != pars1.Length)
                        throw new Exception($"{type}类的:{info.Name}方法定义Rpc的参数长度不一致!");
                    for (int i = 0; i < pars.Length; i++)
                        if(pars[i].ParameterType != pars1[i])
                            throw new Exception($"{type}类的:{info.Name}方法定义Rpc的参数类型不一致!");
                }
#endif
                var parTypes = new Type[pars.Length];
                for (int i = 0; i < pars.Length; i++)
                    parTypes[i] = pars[i].ParameterType;
                Type gt;
                if (parTypes.Length == 0)
                    gt = typeof(RPCPTRNull);
                else
                    gt = Type.GetType($"Net.Adapter.RPCPTR`{pars.Length}").MakeGenericType(parTypes);
                var metPtr = (RPCPTR)Activator.CreateInstance(gt);
                var ptr = metPtr.GetType().GetField("ptr", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                var met = Delegate.CreateDelegate(ptr.FieldType, target, method);
                ptr.SetValue(metPtr, met);
                metPtr.cmd = data.rpc.cmd;
                data.ptr = metPtr;
            }, (member) => new RPCMethodPtr(target, member.member as MethodInfo, member.rpc.cmd, member.ptr));
        }

        public void OnRpcExecute(RPCModel model)
        {
            RpcHelper.Invoke(handle, model, (methods) => 
            {
                foreach (RPCMethodPtr rpc in methods.Values)
                {
                    if (rpc.cmd == NetCmd.ThreadRpc)
                    {
                        rpc.Invoke(model.pars);
                    }
                    else
                    {
                        var data = new RPCDataPtr(rpc.target, rpc.method, rpc.ptr, model.pars);
                        handle.RpcWorkQueue.Enqueue(data);
                    }
                }
            }, log=> {
                switch (log)
                {
                    case 0:
                        NDebug.LogWarning($"[mask:{model.methodHash}]的远程方法未被收集!请定义[Rpc(hash = {model.methodHash})] void xx方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                        break;
                    case 1:
                        NDebug.LogWarning($"{model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                        break;
                    case 2:
                        NDebug.LogWarning($"{model}的远程方法未被收集!请定义[Rpc]void xx方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                        break;
                }
            });
        }

        public void RemoveRpc(object target)
        {
            handle.RemoveRpc(target);
        }

        //public void CheckRpc()
        //{
        //    handle.CheckRpc();
        //}

        public RPCModelTask OnRpcTaskRegister(ushort methodHash, string callbackFunc)
        {
            RPCModelTask model;
            if (methodHash != 0)
            {
                if (!handle.RpcTasks1.TryGetValue(methodHash, out model))
                    handle.RpcTasks1.TryAdd(methodHash, model = new RPCModelTask());
            }
            else
            {
                if (!handle.RpcTasks.TryGetValue(callbackFunc, out model))
                    handle.RpcTasks.TryAdd(callbackFunc, model = new RPCModelTask());
            }
            return model;
        }
    }
}