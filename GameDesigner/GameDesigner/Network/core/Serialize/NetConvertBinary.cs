namespace Net.Serialize
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Text;
#if SERVICE
    using global::System.Runtime.CompilerServices;
    using Microsoft.CSharp.RuntimeBinder;
    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
#endif
    using global::System.Collections;
    using global::System.Runtime.InteropServices;
    using Net.Share;
    using Net.System;

    /// <summary>
    /// 提供序列化二进制类
    /// </summary>
    public class NetConvertBinary : NetConvertBase
    {
        private static MyDictionary<ushort, Type> serializeTypes = new MyDictionary<ushort, Type>();
        private static MyDictionary<Type, ushort> serializeType1s = new MyDictionary<Type, ushort>();
        private static MyDictionary<Type, string[]> serializeOnly = new MyDictionary<Type, string[]>();
        private static MyDictionary<Type, string[]> serializeIgnore = new MyDictionary<Type, string[]>();
        private static Type nonSerialized = typeof(NonSerialized);
        private static MyDictionary<Type, Member[]> map;

        static NetConvertBinary()
        {
            Init();
        }

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            serializeTypes.Clear();
            serializeType1s.Clear();
            serializeOnly.Clear();
            serializeIgnore.Clear();
            AddSerializeBaseType();
            MakeNonSerializedAttribute<NonSerialized>();
            return true;
        }

        public static void MakeNonSerializedAttribute<T>() where T : Attribute
        {
            nonSerialized = typeof(T);
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddSerializeBaseType()
        {
            AddBaseType<short>();
            AddBaseType<int>();
            AddBaseType<long>();
            AddBaseType<ushort>();
            AddBaseType<uint>();
            AddBaseType<ulong>();
            AddBaseType<float>();
            AddBaseType<double>();
            AddBaseType<bool>();
            AddBaseType<char>();
            AddBaseType<string>();
            AddBaseType<byte>();
            AddBaseType<sbyte>();
            AddBaseType<DateTime>();
            AddBaseType<decimal>();
            AddBaseType<DBNull>();
            AddBaseType<Type>();
            //基础序列化数组
            AddBaseType<short[]>();
            AddBaseType<int[]>();
            AddBaseType<long[]>();
            AddBaseType<ushort[]>();
            AddBaseType<uint[]>();
            AddBaseType<ulong[]>();
            AddBaseType<float[]>();
            AddBaseType<double[]>();
            AddBaseType<bool[]>();
            AddBaseType<char[]>();
            AddBaseType<string[]>();
            AddBaseType<byte[]>();
            AddBaseType<sbyte[]>();
            AddBaseType<DateTime[]>();
            AddBaseType<decimal[]>();
            //基础序列化List
            AddBaseType<List<short>>();
            AddBaseType<List<int>>();
            AddBaseType<List<long>>();
            AddBaseType<List<ushort>>();
            AddBaseType<List<uint>>();
            AddBaseType<List<ulong>>();
            AddBaseType<List<float>>();
            AddBaseType<List<double>>();
            AddBaseType<List<bool>>();
            AddBaseType<List<char>>();
            AddBaseType<List<string>>();
            AddBaseType<List<byte>>();
            AddBaseType<List<sbyte>>();
            AddBaseType<List<DateTime>>();
            AddBaseType<List<decimal>>();
            //基础序列化List
            AddBaseType<List<short[]>>();
            AddBaseType<List<int[]>>();
            AddBaseType<List<long[]>>();
            AddBaseType<List<ushort[]>>();
            AddBaseType<List<uint[]>>();
            AddBaseType<List<ulong[]>>();
            AddBaseType<List<float[]>>();
            AddBaseType<List<double[]>>();
            AddBaseType<List<bool[]>>();
            AddBaseType<List<char[]>>();
            AddBaseType<List<string[]>>();
            AddBaseType<List<byte[]>>();
            AddBaseType<List<sbyte[]>>();
            AddBaseType<List<DateTime[]>>();
            AddBaseType<List<decimal[]>>();
            //基础结构类型初始化
            map = new MyDictionary<Type, Member[]>
            {
                { typeof(byte), new Member[] { new Member() { Type = typeof(byte), IsPrimitive = true, TypeCode = TypeCode.Byte } } },
                { typeof(sbyte), new Member[] { new Member() { Type = typeof(sbyte), IsPrimitive = true, TypeCode = TypeCode.SByte } } },
                { typeof(bool), new Member[] { new Member() { Type = typeof(bool), IsPrimitive = true, TypeCode = TypeCode.Boolean } } },
                { typeof(short), new Member[] { new Member() { Type = typeof(short), IsPrimitive = true, TypeCode = TypeCode.Int16 } } },
                { typeof(ushort), new Member[] { new Member() { Type = typeof(ushort), IsPrimitive = true, TypeCode = TypeCode.UInt16 } } },
                { typeof(char), new Member[] { new Member() { Type = typeof(char), IsPrimitive = true, TypeCode = TypeCode.Char } } },
                { typeof(int), new Member[] { new Member() { Type = typeof(int), IsPrimitive = true, TypeCode = TypeCode.Int32 } } },
                { typeof(uint), new Member[] { new Member() { Type = typeof(uint), IsPrimitive = true, TypeCode = TypeCode.UInt32 } } },
                { typeof(long), new Member[] { new Member() { Type = typeof(long), IsPrimitive = true, TypeCode = TypeCode.Int64 } } },
                { typeof(ulong), new Member[] { new Member() { Type = typeof(ulong), IsPrimitive = true, TypeCode = TypeCode.UInt64 } } },
                { typeof(float), new Member[] { new Member() { Type = typeof(float), IsPrimitive = true, TypeCode = TypeCode.Single } } },
                { typeof(double), new Member[] { new Member() { Type = typeof(double), IsPrimitive = true, TypeCode = TypeCode.Double } } },
                { typeof(DateTime), new Member[] { new Member() { Type = typeof(DateTime), IsPrimitive = true, TypeCode = TypeCode.DateTime } } },
                { typeof(decimal), new Member[] { new Member() { Type = typeof(decimal), IsPrimitive = true, TypeCode = TypeCode.Decimal } } },
                { typeof(string), new Member[] { new Member() { Type = typeof(string), IsPrimitive = true, TypeCode = TypeCode.String } } },
            };
            //其他可能用到的
            AddSerializeType<Vector2>();
            AddSerializeType<Vector3>();
            AddSerializeType<Vector4>();
            AddSerializeType<Quaternion>(null, "eulerAngles");
            AddSerializeType<Rect>(new string[] { "x", "y", "width", "height" });
            AddSerializeType<Color>(null, "hex");
            AddSerializeType<Color32>(null, "hex");
            AddSerializeType<UnityEngine.Vector2>();
            AddSerializeType<UnityEngine.Vector3>();
            AddSerializeType<UnityEngine.Vector4>();
            AddSerializeType<UnityEngine.Quaternion>(null, "eulerAngles");
            AddSerializeType<UnityEngine.Rect>(new string[] { "x", "y", "width", "height" });
            AddSerializeType<UnityEngine.Color>(null, "hex");
            AddSerializeType<UnityEngine.Color32>(null, "hex");
            //框架操作同步用到
            AddSerializeType<Operation>();
            AddSerializeType<Operation[]>();
            AddSerializeType<OperationList>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">序列化的类型</typeparam>
        /// <param name="onlyFields">只序列化的字段名称列表</param>
        /// <param name="ignoreFields">不序列化的字段名称列表</param>
        public static void AddSerializeType<T>(string[] onlyFields = default, params string[] ignoreFields)
        {
            AddSerializeType(typeof(T), onlyFields, ignoreFields);
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">序列化的类型</param>
        /// <param name="onlyFields">只序列化的字段名称列表</param>
        /// <param name="ignoreFields">不序列化的字段名称列表</param>
        public static void AddSerializeType(Type type, string[] onlyFields = default, params string[] ignoreFields)
        {
            if (serializeType1s.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            serializeTypes.Add((ushort)serializeTypes.Count, type);
            serializeType1s.Add(type, (ushort)serializeType1s.Count);
            serializeOnly.Add(type, onlyFields);
            serializeIgnore.Add(type, ignoreFields);
            GetMembers(type);
        }

        private static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (serializeType1s.ContainsKey(type))
                return;
            serializeTypes.Add((ushort)serializeTypes.Count, type);
            serializeType1s.Add(type, (ushort)serializeType1s.Count);
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types"></param>
        public static void AddNetworkType(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddSerializeType(type);
            }
        }

        /// <summary>
        /// 添加网络程序集，此方法将会添加获取传入的类的程序集并全部添加
        /// </summary>
        /// <param name="value">传入的类</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkTypeAssembly(Type value)
        {
            foreach (Type type in value.Assembly.GetTypes().Where((t) =>
            {
                return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
            }))
            {
                if (serializeType1s.ContainsKey(type))
                    continue;
                serializeTypes.Add((ushort)serializeTypes.Count, type);
                serializeType1s.Add(type, (ushort)serializeType1s.Count);
            }
        }

        /// <summary>
        /// 添加网络传输程序集， 注意：添加客户端的程序集必须和服务器的程序集必须保持一致， 否则将会出现问题
        /// </summary>
        /// <param name="assemblies">程序集</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkAssembly(Assembly[] assemblies)
        {
            foreach (Assembly assemblie in assemblies)
            {
                foreach (Type type in assemblie.GetTypes().Where((t) =>
                {
                    return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
                }))
                {
                    if (serializeType1s.ContainsKey(type))
                        continue;
                    serializeTypes.Add((ushort)serializeTypes.Count, type);
                    serializeType1s.Add(type, (ushort)serializeType1s.Count);
                }
            }
        }

        /// <summary>
        /// 索引取类型
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <returns></returns>
        private static Type IndexToType(ushort typeIndex)
        {
            if (serializeTypes.TryGetValue(typeIndex, out Type type))
                return type;
            return null;
        }

        /// <summary>
        /// 类型取索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ushort TypeToIndex(Type type)
        {
            if (serializeType1s.TryGetValue(type, out ushort typeHash))
                return typeHash;
            throw new KeyNotFoundException($"没有注册[{type}]类为序列化对象, 请使用NetConvertBinary.AddNetworkType<{type}>()进行注册类型!");
        }

        /// <summary>
        /// 序列化数组实体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="array"></param>
        private unsafe static void WriteArray(Segment stream, IList array, Type itemType, bool recordType, bool ignore)
        {
            int len = array.Count;
            if (len == 0)
                return;
            stream.Write(len);
            var bitLen = ((len - 1) / 8) + 1;
            byte[] bits = new byte[bitLen];
            int strPos = stream.Position;
            stream.Write(bits, 0, bitLen);
            for (int i = 0; i < len; i++)
            {
                var arr1 = array[i];
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (arr1 == null)
                    continue;
                SetBit(ref bits[bitPos], bitInx1 + 1, true);
                if (recordType)
                {
                    var type = arr1.GetType();
                    stream.Write(TypeToIndex(type));
                    WriteObject(stream, type, arr1, recordType, ignore);
                }
                else WriteObject(stream, itemType, arr1, recordType, ignore);
            }
            int strLen = stream.Position;
            stream.Position = strPos;
            stream.Write(bits, 0, bitLen);
            stream.Position = strLen;
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="array"></param>
        private unsafe static void ReadArray(Segment segment, ref IList array, Type itemType, bool recordType, bool ignore)
        {
            var bitLen = ((array.Count - 1) / 8) + 1;
            byte[] bits = segment.Read(bitLen);
            for (int i = 0; i < array.Count; i++)
            {
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (!GetBit(bits[bitPos], (byte)(++bitInx1)))
                    continue;
                if (recordType)
                {
                    var type = IndexToType(segment.ReadUInt16());
                    array[i] = ReadObject(segment, type, recordType, ignore);
                }
                else array[i] = ReadObject(segment, itemType, recordType, ignore);
            }
        }

        public static byte[] Serialize(string func, params object[] pars)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                stream.Write(func);
                foreach (var obj in pars)
                {
                    if (obj == null)
                        continue;
                    var type = obj.GetType();
                    stream.Write(TypeToIndex(type));
                    WriteObject(stream, type, obj, false, false);
                }
                buffer1 = stream.ToArray();
            }
            catch (Exception ex)
            {
                string str = "函数:" + func + " 参数:";
                foreach (var obj in pars)
                    if (obj == null)
                        str += $"[null]";
                    else
                        str += $"[{obj}]";
                NDebug.LogError("序列化:" + str + "方法出错 详细信息:" + ex);
            }
            finally
            {
                BufferPool.Push(stream);
            }
            return buffer1;
        }

        public static byte[] SerializeModel(RPCModel model)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                byte head = 0;
                bool hasFunc = !string.IsNullOrEmpty(model.func);
                bool hasMask = model.methodHash != 0;
                SetBit(ref head, 1, hasFunc);
                SetBit(ref head, 2, hasMask);
                stream.WriteByte(head);
                if (hasFunc) stream.Write(model.func);
                if (hasMask) stream.Write(model.methodHash);
                foreach (var obj in model.pars)
                {
                    var type = obj.GetType();
                    stream.Write(TypeToIndex(type));
                    WriteObject(stream, type, obj, false, false);
                }
                buffer1 = stream.ToArray();
            }
            catch (Exception ex)
            {
                string str = "函数:" + model.func + " 参数:";
                foreach (var obj in model.pars)
                    if (obj == null)
                        str += $"[null]";
                    else
                        str += $"[{obj}]";
                NDebug.LogError("序列化:" + str + "方法出错 详细信息:" + ex);
            }
            finally
            {
                BufferPool.Push(stream);
            }
            return buffer1;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public static Segment Serialize(object obj, bool recordType = false, bool ignore = false)
        {
            var stream = BufferPool.Take();
            try
            {
                if (obj == null)
                    return default;
                Type type = obj.GetType();
                byte[] typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                stream.Write(typeBytes, 0, 2);
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
            }
            return stream;
        }

        /// <summary>
        /// 序列化对象, 不记录反序列化类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Segment SerializeObject(object obj, bool recordType = false, bool ignore = false)
        {
            var stream = BufferPool.Take();
            try
            {
                if (obj == null)
                    return stream;
                var type = obj.GetType();
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally 
            {
                stream.Count = stream.Position;
                stream.Position = 0;
            }
            return stream;
        }

        /// <summary>
        /// 序列化对象, 不记录反序列化类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void SerializeObject(Segment stream, object obj, bool recordType = false, bool ignore = false)
        {
            try
            {
                if (obj == null)
                    return;
                var type = obj.GetType();
                WriteObject(stream, type, obj, recordType, ignore);
            }
            catch (Exception ex)
            {
                NDebug.LogError("序列化:" + obj + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
            }
        }

        private class Member
        {
            internal string name;
            internal bool IsPrimitive;
            internal bool IsEnum;
            internal bool IsArray;
            internal bool IsGenericType;
            internal Type Type;
            internal TypeCode TypeCode;
            internal Type ItemType;
            internal Type ItemType1;
            internal bool IsPrimitive1, IsPrimitive2;
            internal Type[] ItemTypes;
            internal object defaultV;
            internal bool Intricate;
#if !SERVICE
            internal Func<object, object> getValue;
            internal Action<object, object> setValue;
#endif
            internal virtual object GetValue(object obj)
            {
                return obj;
            }

            internal virtual void SetValue(ref object obj, object v)
            {
                obj = v;
            }

            public override string ToString()
            {
                return $"{name} {Type}";
            }
        }
#if SERVICE
        private class FPMember<T> : Member
        {
            internal CallSite<Func<CallSite, object, object>> getValueCall;
            internal CallSite<Func<CallSite, object, T, object>> setValueCall;
            internal override object GetValue(object obj)
            {
                return getValueCall.Target(getValueCall, obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValueCall.Target(setValueCall, obj, (T)v);
            }
        }
        private class FPArrayMember<T> : Member
        {
            internal CallSite<Func<CallSite, object, object>> getValueCall;
            internal CallSite<Func<CallSite, object, T[], object>> setValueCall;
            internal override object GetValue(object obj)
            {
                return getValueCall.Target(getValueCall, obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValueCall.Target(setValueCall, obj, (T[])v);
            }
        }
#else
        private class FPMember : Member
        {
            internal override object GetValue(object obj)
            {
                return getValue(obj);
            }
            internal override void SetValue(ref object obj, object v)
            {
                setValue(obj, v);
            }
        }
#endif
        private static Member[] GetMembers(Type type)
        {
            if (!map.TryGetValue(type, out Member[] members2))
            {
                var members1 = new List<Member>();
                if (type.IsArray | type.IsGenericType) 
                {
                    Member member1 = GetFPMember(null, type, type.FullName, false);
                    members1.Add(member1);
                }
                else
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var members = new List<MemberInfo>(fields);
                    members.AddRange(properties);
                    if (!serializeOnly.TryGetValue(type, out var onlys))
                        onlys = new string[0];
                    else if (onlys == null)
                        onlys = new string[0];
                    if (!serializeIgnore.TryGetValue(type, out var ignores))
                        ignores = new string[0];
                    foreach (var member in members)
                    {
                        if (member.GetCustomAttribute(nonSerialized) != null)
                            continue;
                        if (onlys.Length != 0 & !onlys.Contains(member.Name))
                            continue;
                        if (ignores.Contains(member.Name))
                            continue;
                        Member member1;
                        if (member.MemberType == MemberTypes.Field)
                        {
                            var field = member as FieldInfo;
                            var fType = field.FieldType;
                            if (fType.IsArray)
                            {
                                var arrItemType = fType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                                if (!CanSerialized(arrItemType))
                                    continue;
                            }
                            else if (fType.IsGenericType)
                            {
                                if (fType.GenericTypeArguments.Length == 1)
                                {
                                    var listType = fType.GenericTypeArguments[0];
                                    if (!CanSerialized(listType))
                                        continue;
                                }
                                else if (fType.GenericTypeArguments.Length == 2)
                                {
                                    Type keyType = fType.GenericTypeArguments[0];
                                    Type valueType = fType.GenericTypeArguments[1];
                                    if (!CanSerialized(keyType))
                                        continue;
                                    if (!CanSerialized(valueType))
                                        continue;
                                }
                            }
                            else 
                            {
                                if (!CanSerialized(fType))
                                    continue;
                            }
                            member1 = GetFPMember(type, fType, field.Name, true);
#if !SERVICE
                            member1.getValue = field.GetValue;
                            member1.setValue = field.SetValue;
#endif
                            members1.Add(member1);
                        }
                        else if (member.MemberType == MemberTypes.Property)
                        {
                            var property = member as PropertyInfo;
                            if (!property.CanRead | !property.CanWrite)
                                continue;
                            if (property.GetIndexParameters().Length > 0)
                                continue;
                            var pType = property.PropertyType;
                            if (pType.IsArray)
                            {
                                var arrItemType = pType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                                if (!CanSerialized(arrItemType))
                                    continue;
                            }
                            else if (pType.IsGenericType)
                            {
                                if (pType.GenericTypeArguments.Length == 1)
                                {
                                    var listType = pType.GenericTypeArguments[0];
                                    if (!CanSerialized(listType))
                                        continue;
                                }
                                else if (pType.GenericTypeArguments.Length == 2)
                                {
                                    Type keyType = pType.GenericTypeArguments[0];
                                    Type valueType = pType.GenericTypeArguments[1];
                                    if (!CanSerialized(keyType))
                                        continue;
                                    if (!CanSerialized(valueType))
                                        continue;
                                }
                            }
                            else
                            {
                                if (!CanSerialized(pType))
                                    continue;
                            }
                            member1 = GetFPMember(type, pType, property.Name, true);
#if !SERVICE
                            member1.getValue = property.GetValue;
                            member1.setValue = property.SetValue;
#endif
                            members1.Add(member1);
                        }
                    }
                }
                map.TryAdd(type, members2 = members1.ToArray());
            }
            return members2;
        }

        private static bool CanSerialized(Type type)
        {
            if (type == typeof(Type) | type == typeof(object))
                return false;
            if (type.IsClass & type != typeof(string))
            {
                if (type.IsArray)
                {
                    var arrItemType = type.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                    return CanSerialized(arrItemType);
                }
                else if (type.IsGenericType)
                {
                    if (type.GenericTypeArguments.Length == 1)
                    {
                        var listType = type.GenericTypeArguments[0];
                        return CanSerialized(listType);
                    }
                    else if (type.GenericTypeArguments.Length == 2)
                    {
                        Type keyType = type.GenericTypeArguments[0];
                        Type valueType = type.GenericTypeArguments[1];
                        if (!CanSerialized(keyType))
                            return false;
                        if (!CanSerialized(valueType))
                            return false;
                    }
                }
                var constructors = type.GetConstructors();
                bool hasConstructor = false;
                foreach (var constructor in constructors)
                {
                    if (constructor.GetParameters().Length == 0)
                    {
                        hasConstructor = true;
                        break;
                    }
                }
                if (!hasConstructor)
                    return false;
            }
#if !SERVICE
            if (type.IsSubclassOf(typeof(UnityEngine.Object)) | type == typeof(UnityEngine.Object))
                return false;
#endif
            return true;
        }

        private static Member GetFPMember(Type type, Type fpType, string Name, bool isClassField) 
        {
#if SERVICE
            object getValueCall = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(0, Name, type, new CSharpArgumentInfo[]
            {
                CSharpArgumentInfo.Create(0, null)
            }));
            var csType = typeof(Func<,,,>).MakeGenericType(typeof(CallSite), typeof(object), fpType, typeof(object));
            var setValueCall = CallSite.Create(csType, Binder.SetMember(0, Name, type, new CSharpArgumentInfo[]
            {
                CSharpArgumentInfo.Create(0, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
            }));
            var pm = typeof(FPMember<>).MakeGenericType(new Type[] { fpType });
            var member1 = (Member)Activator.CreateInstance(pm);
#else
            Member member1;
            if (!isClassField)
                member1 = new Member();
            else
                member1 = new FPMember();
#endif
            if (fpType.IsArray)
            {
                Type itemType = fpType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
#if SERVICE
                if (isClassField)
                {
                    var pm1 = typeof(FPArrayMember<>).MakeGenericType(new Type[] { itemType });
                    member1 = (Member)Activator.CreateInstance(pm1);
                    csType = typeof(Func<,,,>).MakeGenericType(typeof(CallSite), typeof(object), fpType, typeof(object));
                    setValueCall = CallSite.Create(csType, Binder.SetMember(0, Name, type, new CSharpArgumentInfo[]
                    {
                        CSharpArgumentInfo.Create(0, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                    }));
                }
                else 
                {
                    member1 = new Member();
                    getValueCall = null;
                    setValueCall = null;
                }
#endif
                member1.ItemType = itemType;
                member1.IsPrimitive1 = Type.GetTypeCode(itemType) != TypeCode.Object;
            }
            else if (fpType.IsGenericType)
            {
#if SERVICE
                if (!isClassField) 
                {
                    member1 = new Member();
                    getValueCall = null;
                    setValueCall = null;
                }
#endif
                if (fpType.GenericTypeArguments.Length == 1)
                {
                    Type itemType = fpType.GenericTypeArguments[0];
                    member1.ItemType = itemType;
                    member1.Intricate = fpType.GetInterface(typeof(IList).Name) == null;
                    member1.IsPrimitive1 = Type.GetTypeCode(itemType) != TypeCode.Object;
                }
                else if (fpType.GenericTypeArguments.Length == 2)
                {
                    Type keyType = fpType.GenericTypeArguments[0];
                    Type valueType = fpType.GenericTypeArguments[1];
                    member1.ItemType = keyType;
                    member1.ItemType1 = valueType;
                    member1.IsPrimitive1 = Type.GetTypeCode(keyType) != TypeCode.Object;
                    if (valueType.IsArray)
                    {
                        var arrItemType = valueType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                        member1.IsPrimitive2 = Type.GetTypeCode(arrItemType) != TypeCode.Object;
                        member1.ItemTypes = new Type[] { arrItemType };
                    }
                    else if (valueType.IsGenericType & valueType.GenericTypeArguments.Length == 1)
                    {
                        var arrItemType = valueType.GenericTypeArguments[0];
                        member1.IsPrimitive2 = Type.GetTypeCode(arrItemType) != TypeCode.Object;
                        member1.ItemTypes = new Type[] { arrItemType };
                    }
                    else 
                    {
                        member1.IsPrimitive2 = Type.GetTypeCode(valueType) != TypeCode.Object;
                        member1.ItemTypes = new Type[] { valueType };
                    }
                }
            }
#if SERVICE
            if (setValueCall != null & getValueCall != null)
            {
                var callS = member1.GetType().GetField("setValueCall", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                callS.SetValue(member1, setValueCall);
                var callS1 = member1.GetType().GetField("getValueCall", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                callS1.SetValue(member1, getValueCall);
            }
#endif
            member1.name = Name;
            member1.IsPrimitive = Type.GetTypeCode(fpType) != TypeCode.Object;//.IsPrimitive | (fpType == typeof(string)) | (fpType == typeof(decimal)) | (fpType == typeof(DateTime));
            member1.IsEnum = fpType.IsEnum;
            member1.IsArray = fpType.IsArray;
            member1.IsGenericType = fpType.IsGenericType;
            member1.Type = fpType;
            member1.TypeCode = Type.GetTypeCode(fpType);
            if (member1.IsPrimitive)
            {
                if (fpType == typeof(string))
                    member1.defaultV = null;
                else
                    member1.defaultV = Activator.CreateInstance(member1.Type);
            }
            return member1;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="recordType"></param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        private static void WriteObject(Segment segment, Type type, object target, bool recordType, bool ignore)
        {
            var members = GetMembers(type);
            var bitLen = ((members.Length - 1) / 8) + 1;
            byte[] bits = new byte[bitLen];
            var strPos = segment.Position;
            segment.Position += bitLen;
            for (byte i = 0; i < members.Length; i++)
            {
                var member = members[i];
                object value = member.GetValue(target);
                if (value == null)
                    continue;
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (member.IsPrimitive)
                {
                    if (!value.Equals(member.defaultV))
                    {
                        segment.WriteValue(value);
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                    }
                }
                else if (member.IsEnum)
                {
                    var enumValue = value.GetHashCode();
                    if (enumValue == 0)
                        continue;
                    segment.Write(enumValue);
                    SetBit(ref bits[bitPos], bitInx1 + 1, true);
                }
                else if (member.IsArray | member.IsGenericType)
                {
                    if (member.ItemType1 == null)
                    {
                        var array = value as IList;
                        if (array == null)
                        {
                            array = Activator.CreateInstance(typeof(List<>).MakeGenericType(member.ItemType)) as IList;
                            var array1 = value as IEnumerable;
                            var enumerator = array1.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                array.Add(enumerator.Current);
                            }
                        }
                        if (array.Count == 0)
                            continue;
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                        if (member.IsPrimitive1)
                        {
                            if (member.IsArray) segment.WriteArray(array);
                            else segment.WriteList(array);
                        }
                        else WriteArray(segment, array, member.ItemType, recordType, ignore);
                    }
                    else
                    {
                        var dict = (IDictionary)value;
                        if (dict.Count == 0)
                            continue;
                        SetBit(ref bits[bitPos], bitInx1 + 1, true);
                        if (!member.IsPrimitive1)
                            throw new Exception("字典Key必须是基础类型！");
                        segment.Write(dict.Count);
                        var enumerator = dict.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            segment.WriteValue(enumerator.Key);
                            if (member.ItemType1.IsArray & member.IsPrimitive2)
                            {
                                segment.WriteArray(enumerator.Value);
                            }
                            else if (member.ItemType1.IsGenericType & member.IsPrimitive2)
                            {
                                segment.WriteList(enumerator.Value);
                            }
                            else if (member.IsPrimitive2)
                            {
                                segment.WriteValue(enumerator.Value);
                            }
                            else WriteObject(segment, member.ItemType1, enumerator.Value, recordType, ignore);
                        }
                    }
                }
                else if (serializeType1s.ContainsKey(member.Type) | ignore)
                {
                    SetBit(ref bits[bitPos], bitInx1 + 1, true);
                    WriteObject(segment, member.Type, value, recordType, ignore);
                }
                else throw new Exception($"你没有标记此类[{member.Type}]为可序列化! 请使用NetConvertBinary.AddNetworkType<T>()方法进行添加此类为可序列化类型!");
            }
            var strLen = segment.Position;
            segment.Position = strPos;
            segment.Write(bits, 0, bitLen);
            segment.Position = strLen;
        }

        public static FuncData Deserialize(byte[] buffer, int index, int count, bool ignore = false)
        {
            FuncData obj = default;
            var segment = new Segment(buffer, index, count, false);
            try
            {
                count += index;
                obj.name = segment.ReadString();
                List<object> list = new List<object>();
                while (segment.Position < segment.Offset + segment.Count)
                {
                    Type type = IndexToType(segment.ReadUInt16());
                    if (type == null)
                        break;
                    index += 2;
                    var obj1 = ReadObject(segment, type, false, ignore);
                    list.Add(obj1);
                }
                obj.pars = list.ToArray();
            }
            catch (Exception ex)
            {
                NDebug.LogError($"解析[{obj.name}]出错 详细信息:" + ex);
                obj.error = true;
            }
            return obj;
        }

        public static FuncData DeserializeModel(byte[] buffer, int index, int count, bool ignore = false)
        {
            FuncData obj = default;
            var segment = new Segment(buffer, index, count, false);
            try
            {
                byte head = segment.ReadByte();
                bool hasFunc = GetBit(head, 1);
                bool hasMask = GetBit(head, 2);
                if(hasFunc) obj.name = segment.ReadString();
                if(hasMask) obj.hash = segment.ReadUInt16();
                var list = new List<object>();
                while (segment.Position < segment.Offset + segment.Count)
                {
                    var type = IndexToType(segment.ReadUInt16());
                    if (type == null)
                        break;
                    var obj1 = ReadObject(segment, type, false, ignore);
                    list.Add(obj1);
                }
                obj.pars = list.ToArray();
            }
            catch (Exception ex)
            {
                NDebug.LogError($"解析[{obj.name}]出错 详细信息:" + ex);
                obj.error = true;
            }
            return obj;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="recordType">序列化的类型字段是 object[]字段时, 可以帮你记录object的绝对类型</param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(byte[] buffer, int index, int count, bool recordType = false, bool ignore = false)
        {
            return DeserializeObject<T>(new Segment(buffer, index, count), default, recordType, ignore);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <param name="recordType"></param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(Segment segment, bool isPush = true, bool recordType = false, bool ignore = false)
        {
            return (T)DeserializeObject(segment, typeof(T), isPush, recordType, ignore);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="recordType"></param>
        /// <param name="ignore">忽略不使用<see cref="AddBaseType"/>方法也会被序列化</param>
        /// <returns></returns>
        public static object DeserializeObject(Segment segment, Type type, bool isPush = true, bool recordType = false, bool ignore = false)
        {
            object obj = default;
            obj = ReadObject(segment, type, recordType, ignore);
            if (isPush) BufferPool.Push(segment);
            return obj;
        }

        public static object Deserialize(Segment segment, bool isPush = true, bool ignore = false)
        {
            object obj = null;
            if (segment.Position < segment.Offset + segment.Count)
            {
                var type = IndexToType(segment.ReadUInt16());
                if (type == null)
                    return obj;
                obj = ReadObject(segment, type, false, ignore);
            }
            if(isPush) BufferPool.Push(segment);
            return obj;
        }

        /// <summary>
        /// 反序列化实体对象
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ReadObject(Segment segment, Type type, bool recordType, bool ignore)
        {
            object obj;
            if (type == typeof(string)) obj = string.Empty;
            else if (type.IsArray) obj = default;
            else obj = Activator.CreateInstance(type);
            var members = GetMembers(type);
            var bitLen = ((members.Length - 1) / 8) + 1;
            byte[] bits = segment.Read(bitLen);
            for (int i = 0; i < members.Length; i++)
            {
                int bitInx1 = i % 8;
                int bitPos = i / 8;
                if (!GetBit(bits[bitPos], (byte)(++bitInx1)))
                    continue;
                var member = members[i];
                if (member.IsPrimitive)//如果是基础类型
                {
                    member.SetValue(ref obj, segment.ReadValue(member.TypeCode));
                }
                else if (member.IsEnum)//如果是枚举类型
                {
                    member.SetValue(ref obj, segment.ReadEnum(member.Type));
                }
                else if (member.IsArray | member.IsGenericType)
                {
                    if (member.ItemType1 == null)//如果itemType1是空的话，说明是List类型，否则是字典
                    {
                        IList array;
                        if (member.IsPrimitive1)
                        {
                            if (member.IsArray)
                                array = (IList)segment.ReadArray(member.ItemType);
                            else
                                array = (IList)segment.ReadList(member.ItemType);
                        }
                        else
                        {
                            if (member.IsArray)
                            {
                                int arrCount = segment.ReadInt32();
                                array = (IList)Activator.CreateInstance(member.Type, arrCount);
                                ReadArray(segment, ref array, member.ItemType, recordType, ignore);
                            }
                            else
                            {
                                int arrCount = segment.ReadInt32();
                                var array1 = Array.CreateInstance(member.ItemType, arrCount);
                                array = (IList)Activator.CreateInstance(member.Type, array1);
                                ReadArray(segment, ref array, member.ItemType, recordType, ignore);
                            }
                        }
                        if (!member.Intricate)
                        {
                            member.SetValue(ref obj, array);
                        }
                        else 
                        {
                            var array1 = Activator.CreateInstance(member.Type);
                            var addMethod = member.Type.GetMethod("Add", new Type[] {  member.ItemType });
                            foreach (var item in array)
                            {
                                addMethod.Invoke(array1, new object[] { item });
                            }
                            member.SetValue(ref obj, array1);
                        }
                    }
                    else
                    {
                        var dictCount = segment.ReadInt32();
                        var dict = (IDictionary)Activator.CreateInstance(member.Type);
                        for (int a = 0; a < dictCount; a++)
                        {
                            var key = segment.ReadValue(member.ItemType);
                            object value;
                            if (member.ItemType1.IsArray & member.IsPrimitive2)
                            {
                                value = segment.ReadArray(member.ItemTypes[0]);
                            }
                            else if (member.ItemType1.IsGenericType & member.IsPrimitive2)
                            {
                                value = segment.ReadList(member.ItemTypes[0]);
                            }
                            else if (member.IsPrimitive2)
                            {
                                value = segment.ReadValue(member.ItemType1);
                            }
                            else value = ReadObject(segment, member.ItemType1, recordType, ignore);
                            dict.Add(key, value);
                        }
                        member.SetValue(ref obj, dict);
                    }
                }
                else if (serializeType1s.ContainsKey(member.Type) | ignore)//如果是序列化类型
                {
                    member.SetValue(ref obj, ReadObject(segment, member.Type, recordType, ignore));
                }
                else throw new Exception($"你没有标记此类[{member.Type}]为可序列化! 请使用NetConvertBinary.AddNetworkType<T>()方法进行添加此类为可序列化类型!");
            }
            return obj;
        }
    }
}