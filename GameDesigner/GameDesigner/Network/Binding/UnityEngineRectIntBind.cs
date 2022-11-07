using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct UnityEngineRectIntBind : ISerialize<UnityEngine.RectInt>, ISerialize
    {
        public void Write(UnityEngine.RectInt value, Segment stream)
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

            if (value.width != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.width);
            }

            if (value.height != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.height);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public UnityEngine.RectInt Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.RectInt();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.width = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.height = stream.ReadInt32();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((UnityEngine.RectInt)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct UnityEngineRectIntArrayBind : ISerialize<UnityEngine.RectInt[]>, ISerialize
	{
		public void Write(UnityEngine.RectInt[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectIntBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.RectInt[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new UnityEngine.RectInt[count];
			if (count == 0) return value;
			var bind = new UnityEngineRectIntBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.RectInt[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct UnityEngineRectIntGenericBind : ISerialize<List<UnityEngine.RectInt>>, ISerialize
	{
		public void Write(List<UnityEngine.RectInt> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectIntBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.RectInt> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<UnityEngine.RectInt>(count);
			if (count == 0) return value;
			var bind = new UnityEngineRectIntBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.RectInt>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}