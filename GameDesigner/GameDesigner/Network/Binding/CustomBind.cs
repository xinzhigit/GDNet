using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
    /// <summary>
    /// 基础类型绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BaseBind<T> : ISerialize<T>, ISerialize
    {
        public void Write(T value, Segment stream)
        {
            stream.WriteValue(value);
        }
        public T Read(Segment stream)
        {
            return stream.ReadValue<T>();
        }

        public void WriteValue(object value, Segment stream)
        {
            stream.WriteValue(value);
        }

        object ISerialize.ReadValue(Segment stream)
        {
            return stream.ReadValue<T>();
        }
    }

    /// <summary>
    /// 基础类型数组绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BaseArrayBind<T> : ISerialize<T[]>, ISerialize
    {
        public void Write(T[] value, Segment stream)
        {
            stream.WriteArray(value);
        }
        public T[] Read(Segment stream)
        {
            return stream.ReadArray<T>();
        }

        public void WriteValue(object value, Segment stream)
        {
            stream.WriteArray(value);
        }

        object ISerialize.ReadValue(Segment stream)
        {
            return stream.ReadArray<T>();
        }
    }

    /// <summary>
    /// 基础类型泛型绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BaseListBind<T> : ISerialize<List<T>>, ISerialize
    {
        public void Write(List<T> value, Segment stream)
        {
            stream.WriteList(value);
        }
        public List<T> Read(Segment stream)
        {
            return stream.ReadList<T>();
        }

        public void WriteValue(object value, Segment stream)
        {
            stream.WriteList(value);
        }

        object ISerialize.ReadValue(Segment stream)
        {
            return stream.ReadList<T>();
        }
    }

    /// <summary>
    /// 字典绑定
    /// </summary>
    public struct DictionaryBind<TKey, TValue>
	{
		public void Write(Dictionary<TKey, TValue> value, Segment stream, ISerialize<TValue> bind)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			foreach (var value1 in value)
			{
				stream.WriteValue(value1.Key);
				bind.Write(value1.Value, stream);
			}
		}

		public Dictionary<TKey, TValue> Read(Segment stream, ISerialize<TValue> bind)
		{
			var count = stream.ReadInt32();
			var value = new Dictionary<TKey, TValue>();
			if (count == 0) return value;
			for (int i = 0; i < count; i++)
			{
				var key = stream.ReadValue<TKey>();
				var tvalue = bind.Read(stream);
				value.Add(key, tvalue);
			}
			return value;
		}
	}

    /// <summary>
    /// My字典绑定
    /// </summary>
    public struct MyDictionaryBind<TKey, TValue>
    {
        public void Write(MyDictionary<TKey, TValue> value, Segment stream, ISerialize<TValue> bind)
        {
            int count = value.Count;
            stream.Write(count);
            if (count == 0) return;
            foreach (var value1 in value)
            {
                stream.WriteValue(value1.Key);
                bind.Write(value1.Value, stream);
            }
        }

        public MyDictionary<TKey, TValue> Read(Segment stream, ISerialize<TValue> bind)
        {
            var count = stream.ReadInt32();
            var value = new MyDictionary<TKey, TValue>();
            if (count == 0) return value;
            for (int i = 0; i < count; i++)
            {
                var key = stream.ReadValue<TKey>();
                var tvalue = bind.Read(stream);
                value.Add(key, tvalue);
            }
            return value;
        }
    }
}