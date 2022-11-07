namespace Net.Serialize
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using Net.Share;
    using Net.System;

    /// <summary>
    /// 快速解析类型, 使用此类需要使用AddNetworkType()先添加序列化类型, 类型是固定, 并且双端统一
    /// </summary>
    public class NetConvertFast : NetConvertBase
    {
        private static readonly Dictionary<ushort, Type> Types = new Dictionary<ushort, Type>();
        private static readonly Dictionary<Type, ushort> Types1 = new Dictionary<Type, ushort>();

        static NetConvertFast()
        {
            Init();
        }

        public static void Init()
        {
            Types.Clear();
            Types1.Clear();
            AddNetworkBaseType();
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddNetworkBaseType()
        {
            AddSerializeType<short>();
            AddSerializeType<int>();
            AddSerializeType<long>();
            AddSerializeType<ushort>();
            AddSerializeType<uint>();
            AddSerializeType<ulong>();
            AddSerializeType<float>();
            AddSerializeType<double>();
            AddSerializeType<bool>();
            AddSerializeType<char>();
            AddSerializeType<string>();
            AddSerializeType<byte>();
            AddSerializeType<sbyte>();
            AddSerializeType<DateTime>();
            AddSerializeType<decimal>();
            AddSerializeType<DBNull>();
            AddSerializeType<Type>();
            //基础序列化数组
            AddSerializeType<short[]>();
            AddSerializeType<int[]>();
            AddSerializeType<long[]>();
            AddSerializeType<ushort[]>();
            AddSerializeType<uint[]>();
            AddSerializeType<ulong[]>();
            AddSerializeType<float[]>();
            AddSerializeType<double[]>();
            AddSerializeType<bool[]>();
            AddSerializeType<char[]>();
            AddSerializeType<string[]>();
            AddSerializeType<byte[]>();
            AddSerializeType<sbyte[]>();
            AddSerializeType<DateTime[]>();
            AddSerializeType<decimal[]>();
            //基础序列化List
            AddSerializeType<List<short>>();
            AddSerializeType<List<int>>();
            AddSerializeType<List<long>>();
            AddSerializeType<List<ushort>>();
            AddSerializeType<List<uint>>();
            AddSerializeType<List<ulong>>();
            AddSerializeType<List<float>>();
            AddSerializeType<List<double>>();
            AddSerializeType<List<bool>>();
            AddSerializeType<List<char>>();
            AddSerializeType<List<string>>();
            AddSerializeType<List<byte>>();
            AddSerializeType<List<sbyte>>();
            AddSerializeType<List<DateTime>>();
            AddSerializeType<List<decimal>>();
            //基础序列化List
            AddSerializeType<List<short[]>>();
            AddSerializeType<List<int[]>>();
            AddSerializeType<List<long[]>>();
            AddSerializeType<List<ushort[]>>();
            AddSerializeType<List<uint[]>>();
            AddSerializeType<List<ulong[]>>();
            AddSerializeType<List<float[]>>();
            AddSerializeType<List<double[]>>();
            AddSerializeType<List<bool[]>>();
            AddSerializeType<List<char[]>>();
            AddSerializeType<List<string[]>>();
            AddSerializeType<List<byte[]>>();
            AddSerializeType<List<sbyte[]>>();
            AddSerializeType<List<DateTime[]>>();
            AddSerializeType<List<decimal[]>>();
            //其他可能用到的
            AddSerializeType<Vector2>();
            AddSerializeType<Vector3>();
            AddSerializeType<Vector4>();
            AddSerializeType<Quaternion>();
            AddSerializeType<Rect>();
            AddSerializeType<Color>();
            AddSerializeType<Color32>();
            AddSerializeType<UnityEngine.Vector2>();
            AddSerializeType<UnityEngine.Vector3>();
            AddSerializeType<UnityEngine.Vector4>();
            AddSerializeType<UnityEngine.Quaternion>();
            AddSerializeType<UnityEngine.Rect>();
            AddSerializeType<UnityEngine.Color>();
            AddSerializeType<UnityEngine.Color32>();
            //框架操作同步用到
            AddSerializeType<Operation>();
            AddSerializeType<Operation[]>();
            AddSerializeType<OperationList>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        public static void AddSerializeType<T>()
        {
            AddSerializeType(typeof(T));
        }

        /// <summary>
        /// 添加经过网络传送的类型
        /// </summary>
        /// <param name="type"></param>
        public static void AddSerializeType(Type type)
        {
            if (Types1.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            Types.Add((byte)Types.Count, type);
            Types1.Add(type, (byte)Types1.Count);
        }

        /// <summary>
        /// 添加网络传输的程序集, 程序集内的所有类型都会被添加, 注意: 客户端和服务器都必须统一使用一模一样的程序集, 否则有可能出现问题!
        /// </summary>
        /// <param name="assembly"></param>
        public static void AddAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes().Where(t => { return !t.IsAbstract; }))
            {
                AddSerializeType(type);
            }
        }

        /// <summary>
        /// 添加assembly程序集的所有nameSpace命名空间的类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        public static void AddNameSpaceTypes(Assembly assembly, string nameSpace)
        {
            foreach (Type type in assembly.GetTypes().Where(t => { return !t.IsAbstract & t.Namespace == nameSpace; }))
            {
                AddSerializeType(type);
            }
        }

        public static byte[] Serialize(RPCModel model)
        {
            var segment = BufferPool.Take();
            byte head = 0;
            bool hasFunc = !string.IsNullOrEmpty(model.func);
            bool hasMask = model.methodHash != 0;
            SetBit(ref head, 1, hasFunc);
            SetBit(ref head, 2, hasMask);
            segment.WriteByte(head);
            if (hasFunc)
                segment.Write(model.func);
            if (hasMask)
                segment.Write(model.methodHash);
            foreach (object obj in model.pars)
            {
                var type = obj.GetType();
                segment.Write(GetTypeHash(type));
                NetConvertBinary.SerializeObject(segment, obj, false, true);
            }
            return segment.ToArray(true);
        }

        private static ushort GetTypeHash(Type type)
        {
            if (Types1.TryGetValue(type, out var typeHash))
                return typeHash;
            throw new IOException($"参数类型:[{type}]没有被注册! 请使用NetConvertFast.AddNetworkType<{type}>()添加序列化类型! 双端都要添加");
        }

        public static Type GetTypeHash(ushort hashCode)
        {
            if (Types.TryGetValue(hashCode, out Type type))
                return type;
            NDebug.LogError($"找不到哈希代码类型:{hashCode}, 类型太复杂时需要使用 NetConvertOld.AddSerializeType(type) 添加类型后再进行系列化!");
            return null;
        }

        public static FuncData Deserialize(byte[] buffer, int index, int count)
        {
            FuncData fdata = default;
            try
            {
                var segment = new Segment(buffer, index, count, false);
                byte head = segment.ReadByte();
                bool hasFunc = GetBit(head, 1);
                bool hasMask = GetBit(head, 2);
                if (hasFunc)
                    fdata.name = segment.ReadString();
                if (hasMask)
                    fdata.hash = segment.ReadUInt16();
                var list = new List<object>();
                while (segment.Position < segment.Offset + segment.Count)
                {
                    var typeName = segment.ReadUInt16();
                    var type = GetTypeHash(typeName);
                    if (type == null)
                    {
                        fdata.error = true;
                        break;
                    }
                    var obj = NetConvertBinary.DeserializeObject(segment, type, false, false, true);
                    list.Add(obj);
                }
                fdata.pars = list.ToArray();
            }
            catch (Exception ex)
            {
                fdata.error = true;
                NDebug.LogError("反序列化:" + ex.ToString());
            }
            return fdata;
        }
    }
}