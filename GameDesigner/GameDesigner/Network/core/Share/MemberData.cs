using Net.Adapter;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Share
{
    public class MemberData
    {
        public MemberInfo member;
        public RPCFun rpc;
        public SyncVarInfo syncVar;
        public RPCPTR ptr;

        public override string ToString()
        {
            return $"{member}";
        }
    }

    public class MemberDataList
    {
        public List<MemberData> members = new List<MemberData>();
    }

    public struct RPCMethodPtr : IRPCMethod
    {
        public byte cmd { get; set; }
        public object target { get; set; }
        public MethodInfo method { get; set; }
        public RPCPTR ptr;

        public RPCMethodPtr(object target, MethodInfo method, byte cmd, RPCPTR ptr) : this()
        {
            this.target = target;
            this.cmd = cmd;
            this.ptr = ptr;
            this.method = method;
        }

        public void Invoke()
        {
            ptr.Invoke(target, null);
        }

        public void Invoke(params object[] pars)
        {
            ptr.Invoke(target, pars);
        }
    }

    public struct RPCDataPtr : IRPCData 
    {
        /// <summary>
        /// 函数和参数的名称
        /// </summary>
        public string name;
        /// <summary>
        /// 存储封包反序列化出来的对象
        /// </summary>
        public object target { get; set; }
        /// <summary>
        /// 存储反序列化的函数
        /// </summary>
        public MethodInfo method { get; set; }
        public RPCPTR ptr;
        /// <summary>
        /// 存储反序列化参数
        /// </summary>
        public object[] pars;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">远程调用对象</param>
        /// <param name="method">远程调用方法</param>
        /// <param name="pars">远程调用参数</param>
        public RPCDataPtr(object target, MethodInfo method, RPCPTR ptr, params object[] pars)
        {
            name = method.ToString();
            this.target = target;
            this.method = method;
            this.pars = pars;
            this.ptr = ptr;
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <returns></returns>
        public void Invoke()
        {
            if (ptr == null)
                return;
            ptr.Invoke(target, pars);
        }

        public override string ToString()
        {
            return $"{target}->{name}";
        }
    }
}