using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct UnityEngineRectOffsetBind : ISerialize<UnityEngine.RectOffset>, ISerialize
    {
        public void Write(UnityEngine.RectOffset value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            byte[] bits = new byte[1];

            if (value.left != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.left);
            }

            if (value.right != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.right);
            }

            if (value.top != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.top);
            }

            if (value.bottom != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.bottom);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public UnityEngine.RectOffset Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.RectOffset();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.left = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.right = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.top = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.bottom = stream.ReadInt32();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((UnityEngine.RectOffset)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct UnityEngineRectOffsetArrayBind : ISerialize<UnityEngine.RectOffset[]>, ISerialize
	{
		public void Write(UnityEngine.RectOffset[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectOffsetBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.RectOffset[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new UnityEngine.RectOffset[count];
			if (count == 0) return value;
			var bind = new UnityEngineRectOffsetBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.RectOffset[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct UnityEngineRectOffsetGenericBind : ISerialize<List<UnityEngine.RectOffset>>, ISerialize
	{
		public void Write(List<UnityEngine.RectOffset> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectOffsetBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.RectOffset> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<UnityEngine.RectOffset>(count);
			if (count == 0) return value;
			var bind = new UnityEngineRectOffsetBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.RectOffset>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}