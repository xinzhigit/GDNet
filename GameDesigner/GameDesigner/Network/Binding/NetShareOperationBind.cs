using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct NetShareOperationBind : ISerialize<Net.Share.Operation>, ISerialize
    {
        public void Write(Net.Share.Operation value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 2;
            byte[] bits = new byte[2];

            if (value.cmd != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.cmd);
            }

            if (value.cmd1 != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.cmd1);
            }

            if (value.cmd2 != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.cmd2);
            }

            if (!string.IsNullOrEmpty(value.name))
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.name);
            }

			if(value.position != default)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				var bind = new NetVector3Bind();
				bind.Write(value.position, stream);
			}

			if(value.rotation != default(Net.Quaternion))
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				var bind = new NetQuaternionBind();
				bind.Write(value.rotation, stream);
			}

			if(value.direction != default)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				var bind = new NetVector3Bind();
				bind.Write(value.direction, stream);
			}

            if (value.identity != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 8, true);
                stream.Write(value.identity);
            }

            if (value.uid != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 1, true);
                stream.Write(value.uid);
            }

            if (value.index != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 2, true);
                stream.Write(value.index);
            }

            if (value.index1 != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 3, true);
                stream.Write(value.index1);
            }

            if (value.index2 != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 4, true);
                stream.Write(value.index2);
            }

            if (value.index3 != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 5, true);
                stream.Write(value.index3);
            }

            if (value.buffer != null)
            {
                NetConvertBase.SetBit(ref bits[1], 6, true);
                stream.Write(value.buffer);
            }

            if (!string.IsNullOrEmpty(value.name1))
            {
                NetConvertBase.SetBit(ref bits[1], 7, true);
                stream.Write(value.name1);
            }

            if (!string.IsNullOrEmpty(value.name2))
            {
                NetConvertBase.SetBit(ref bits[1], 8, true);
                stream.Write(value.name2);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 2);
            stream.Position = pos1;
        }
		
		public Net.Share.Operation Read(Segment stream)
		{
			byte[] bits = stream.Read(2);
			var value = new Net.Share.Operation();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.cmd = stream.ReadByte();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.cmd1 = stream.ReadByte();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.cmd2 = stream.ReadByte();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.name = stream.ReadString();

			if(NetConvertBase.GetBit(bits[0], 5))
			{
				var bind = new NetVector3Bind();
				value.position = bind.Read(stream);
			}

			if(NetConvertBase.GetBit(bits[0], 6))
			{
				var bind = new NetQuaternionBind();
				value.rotation = bind.Read(stream);
			}

			if(NetConvertBase.GetBit(bits[0], 7))
			{
				var bind = new NetVector3Bind();
				value.direction = bind.Read(stream);
			}

			if(NetConvertBase.GetBit(bits[0], 8))
				value.identity = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 1))
				value.uid = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 2))
				value.index = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 3))
				value.index1 = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 4))
				value.index2 = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 5))
				value.index3 = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 6))
				value.buffer = stream.ReadByteArray();

			if(NetConvertBase.GetBit(bits[1], 7))
				value.name1 = stream.ReadString();

			if(NetConvertBase.GetBit(bits[1], 8))
				value.name2 = stream.ReadString();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((Net.Share.Operation)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct NetShareOperationArrayBind : ISerialize<Net.Share.Operation[]>, ISerialize
	{
		public void Write(Net.Share.Operation[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Share.Operation[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new Net.Share.Operation[count];
			if (count == 0) return value;
			var bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Share.Operation[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct NetShareOperationGenericBind : ISerialize<List<Net.Share.Operation>>, ISerialize
	{
		public void Write(List<Net.Share.Operation> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Share.Operation> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<Net.Share.Operation>(count);
			if (count == 0) return value;
			var bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Share.Operation>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}