using System;
using System.Collections.Generic;

namespace Net.System
{
    /// <summary>
    /// 数据缓冲内存池
    /// </summary>
    public static class BufferPool
    {
        /// <summary>
        /// 数据缓冲池大小. 默认65536字节
        /// </summary>
        public static int Size { get; set; } = 65536;
        /// <summary>
        /// 当没有合理回收内存，导致内存泄漏被回收后提示
        /// </summary>
        public static bool Log { get; set; }

        private static readonly GStack<Segment>[] STACKS = new GStack<Segment>[37];
        private static readonly int[] TABLE = new int[] {
            256,512,1024,2048,4096,8192,16384,32768,65536,98304,131072,196608,262144,393216,524288,786432,1048576,
            1572864,2097152,3145728,4194304,6291456,8388608,12582912,16777216,25165824,33554432,50331648,67108864,
            100663296,134217728,201326592,268435456,402653184,536870912,805306368,1073741824
        };

        private static object SyncRoot = new object();

        static BufferPool()
        {
            for (int i = 0; i < TABLE.Length; i++)
            {
                STACKS[i] = new GStack<Segment>();
            }
            ThreadManager.Invoke("BufferPool", 5f, ()=>
            {
                try
                {
                    for (int i = 0; i < STACKS.Length; i++)
                    {
                        foreach (var stack in STACKS[i])
                        {
                            if (stack == null)
                                continue;
                            if (stack.referenceCount == 0) 
                                stack.Close();
                            stack.referenceCount = 0;
                        }
                    }
                }
                catch { }
                return true;
            }, true);
        }

        /// <summary>
        /// 从内存池取数据片
        /// </summary>
        /// <returns></returns>
        public static Segment Take()
        {
            return Take(Size);
        }

        /// <summary>
        /// 从内存池取数据片
        /// </summary>
        /// <param name="size">内存大小</param>
        /// <returns></returns>
        public static Segment Take(int size)
        {
            lock (SyncRoot) 
            {
                var tableInx = 0;
                for (int i = 0; i < TABLE.Length; i++)
                {
                    if (size <= TABLE[i])
                    {
                        size = TABLE[i];
                        tableInx = i;
                        goto J;
                    }
                }
            J: var stack = STACKS[tableInx];
                Segment segment;
            J1: if (stack.Count > 0)
                {
                    segment = stack.Pop();
                    if (!segment.isRecovery | !segment.isDespose)
                        goto J1;
                    goto J2;
                }
                segment = new Segment(new byte[size], 0, size);
            J2: segment.isDespose = false;
                segment.Offset = 0;
                segment.Count = 0;
                segment.Position = 0;
                segment.referenceCount++;
                return segment;
            }
        }

        /// <summary>
        /// 压入数据片, 等待复用
        /// </summary>
        /// <param name="segment"></param>
        public static void Push(Segment segment) 
        {
            lock (SyncRoot) 
            {
                if (!segment.isRecovery)
                    return;
                if (segment.isDespose)
                    return;
                segment.isDespose = true;
                for (int i = 0; i < TABLE.Length; i++)
                {
                    if (segment.length == TABLE[i])
                    {
                        STACKS[i].Push(segment);
                        return;
                    }
                }
            }
        }
    }

    public static class ObjectPool<T> where T : new()
    {
        private static readonly GStack<T> STACK = new GStack<T>();

        public static void Init(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                STACK.Push(new T());
            }
        }

        public static T Take()
        {
            lock (STACK) 
            {
                if (STACK.TryPop(out T obj))
                    return obj;
                return new T();
            }
        }

        public static T Take(Action<T> onNew)
        {
            lock (STACK)
            {
                if (STACK.TryPop(out T obj))
                    return obj;
                obj = new T();
                onNew?.Invoke(obj);
                return obj;
            }
        }

        public static void Push(T obj)
        {
            lock (STACK)
            {
                STACK.Push(obj);
            }
        }

        public static void Clear() 
        {
            lock (STACK)
            {
                STACK.Clear();
            }
        }
    }
}
