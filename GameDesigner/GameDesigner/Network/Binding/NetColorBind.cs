using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct NetColorBind : ISerialize<Net.Color>, ISerialize
    {
        public void Write(Net.Color value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            byte[] bits = new byte[1];

            if (value.r != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.r);
            }

            if (value.g != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.g);
            }

            if (value.b != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.b);
            }

            if (value.a != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.a);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public Net.Color Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Color();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.r = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.g = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.b = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.a = stream.ReadSingle();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((Net.Color)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct NetColorArrayBind : ISerialize<Net.Color[]>, ISerialize
	{
		public void Write(Net.Color[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetColorBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Color[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new Net.Color[count];
			if (count == 0) return value;
			var bind = new NetColorBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Color[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct NetColorGenericBind : ISerialize<List<Net.Color>>, ISerialize
	{
		public void Write(List<Net.Color> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetColorBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Color> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<Net.Color>(count);
			if (count == 0) return value;
			var bind = new NetColorBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Color>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}