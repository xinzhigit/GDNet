using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct UnityEngineVector4Bind : ISerialize<UnityEngine.Vector4>, ISerialize
    {
        public void Write(UnityEngine.Vector4 value, Segment stream)
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
		
		public UnityEngine.Vector4 Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.Vector4();

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
            Write((UnityEngine.Vector4)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct UnityEngineVector4ArrayBind : ISerialize<UnityEngine.Vector4[]>, ISerialize
	{
		public void Write(UnityEngine.Vector4[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineVector4Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Vector4[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new UnityEngine.Vector4[count];
			if (count == 0) return value;
			var bind = new UnityEngineVector4Bind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Vector4[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct UnityEngineVector4GenericBind : ISerialize<List<UnityEngine.Vector4>>, ISerialize
	{
		public void Write(List<UnityEngine.Vector4> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineVector4Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Vector4> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<UnityEngine.Vector4>(count);
			if (count == 0) return value;
			var bind = new UnityEngineVector4Bind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.Vector4>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}