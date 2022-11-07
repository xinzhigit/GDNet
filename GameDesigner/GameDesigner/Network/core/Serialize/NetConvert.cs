namespace Net.Serialize
{
    using Net.Event;
    using global::System;
    using global::System.Collections.Generic;
    using Net.Share;
    using Net.System;

    /// <summary>
    /// 网络转换核心 2019.7.16
    /// </summary>
    public class NetConvert : NetConvertBase
    {
        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="model">函数名</param>
        /// <returns></returns>
        public static byte[] Serialize(RPCModel model, byte[] flag = null)
        {
            var segment = BufferPool.Take();
            byte head = 0;
            bool hasFunc = !string.IsNullOrEmpty(model.func);
            bool hasMask = model.methodHash != 0;
            SetBit(ref head, 1, hasFunc);
            SetBit(ref head, 2, hasMask);
            if (flag != null) segment.Write(flag, 0, flag.Length);
            segment.WriteByte(head);
            if (hasFunc)
                segment.Write(model.func);
            if (hasMask) 
                segment.Write(model.methodHash);
            foreach (object obj in model.pars)
            {
                var type = obj.GetType();
                segment.Write(type.ToString());
                NetConvertBinary.SerializeObject(segment, obj, false, true);
            }
            return segment.ToArray(true);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static FuncData Deserialize(byte[] buffer)
        {
            return Deserialize(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
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
                    var typeName = segment.ReadString();
                    var type = NetConvertOld.GetTypeAll(typeName);
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