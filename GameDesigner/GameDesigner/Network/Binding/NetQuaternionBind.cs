using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct NetQuaternionBind : ISerialize<Net.Quaternion>, ISerialize
    {
        public void Write(Net.Quaternion value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            byte[] bits = new byte[1];

            if (value.x != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.x);
            }

            if (value.y != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.y);
            }

            if (value.z != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.z);
            }

            if (value.w != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.w);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public Net.Quaternion Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Quaternion();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.z = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.w = stream.ReadSingle();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((Net.Quaternion)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct NetQuaternionArrayBind : ISerialize<Net.Quaternion[]>, ISerialize
	{
		public void Write(Net.Quaternion[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Quaternion[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new Net.Quaternion[count];
			if (count == 0) return value;
			var bind = new NetQuaternionBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Quaternion[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct NetQuaternionGenericBind : ISerialize<List<Net.Quaternion>>, ISerialize
	{
		public void Write(List<Net.Quaternion> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Quaternion> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<Net.Quaternion>(count);
			if (count == 0) return value;
			var bind = new NetQuaternionBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Quaternion>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}