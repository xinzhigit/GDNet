//using Net.Event;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Threading;

//namespace Net.System
//{
//    /// <summary>
//    /// 共享文件流, 此类会瓜分一个文件流的段数据作为数据缓存位置
//    /// </summary>
//    public class BufferStream
//    {
//        public Stream stream;
//        public long position;
//        public long Length;
//        internal long offset;
//        internal bool isDispose;
//        internal int referenceCount;

//        public long Position => position;
//        private static readonly object SyncRoot = new object();

//        public void Write(byte[] buffer, int index, int count)
//        {
//            if (position > Length)
//            {
//                NDebug.LogError($"数据缓存超出总长:{position}/{Length}, 如果是大数据请设置BufferStreamShare.Size");
//                return;
//            }
//            lock (SyncRoot)
//            {
//                stream.Seek(offset + position, SeekOrigin.Begin);
//                stream.Write(buffer, index, count);
//            }
//            position += count;
//        }

//        public void Read(byte[] buffer, int index, int count)
//        {
//            lock (SyncRoot)
//            {
//                stream.Seek(offset + position, SeekOrigin.Begin);
//                stream.Read(buffer, index, count);
//            }
//            position += count;
//        }

//        public void Seek(long offset, SeekOrigin origin)
//        {
//            position = offset;
//        }

//        public void Close()
//        {
//            BufferStreamShare.Push(this);
//        }

//        public void Destroy()
//        {
//            stream = null;
//        }

//        ~BufferStream()
//        {
//            BufferStreamShare.Push(this);
//        }
//    }

//    /// <summary>
//    /// 共享文件流类
//    /// </summary>
//    public static class BufferStreamShare
//    {
//        private static readonly string filePath;
//        private static Stream STREAM;
//        private static long currPos;
//        public static long Size = 1024 * 1024;
//        private static readonly GStack<BufferStream> Stack = new GStack<BufferStream>();
//        public static bool UseMemoryStream { get => Net.Config.Config.UseMemoryStream; set => Net.Config.Config.UseMemoryStream = value; }
//        public static int BaseCapacity { get; set; } = 2048;
//        private readonly static object SyncRoot = new object();

//#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
//            [UnityEngine.RuntimeInitializeOnLoadMethod]
//            static void Init()//有了这个方法就行, 在unity初始化就会进入此类的静态构造函数即可
//            {
//            }
//#endif

//        static BufferStreamShare()
//        {
//            var path = Net.Config.Config.GetBasePath();
//            var files = Directory.GetFiles(path, "*.stream");
//            foreach (var file in files)
//                try { File.Delete(file); } catch { }//尝试删除没用的之前的共享文件流
//            filePath = path + $"/{Process.GetCurrentProcess().Id}.stream";
//            if (UseMemoryStream)
//                STREAM = new MemoryStream(BaseCapacity);
//            else
//                STREAM = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
//            ThreadManager.Invoke("BufferStreamSharePool", 10f, () =>
//            {
//                try
//                {
//                    foreach (var stack in Stack)
//                    {
//                        if (stack == null)
//                            continue;
//                        if (stack.referenceCount == 0)
//                            stack.Destroy();
//                        stack.referenceCount = 0;
//                    }
//                }
//                catch { }
//                return true;
//            }, true);
//        }

//        public static BufferStream Take()
//        {
//            lock (SyncRoot)
//            {
//                BufferStream bStream;
//            J: if (Stack.Count == 0)
//                {
//                    bStream = new BufferStream
//                    {
//                        offset = currPos,
//                        Length = Size
//                    };
//                    if (UseMemoryStream)
//                        STREAM = new MemoryStream(BaseCapacity);
//                    else
//                        currPos += Size;
//                    bStream.stream = BufferStreamShare.STREAM;
//                    return bStream;
//                }
//                bStream = Stack.Pop();
//                if (bStream.stream == null)
//                    goto J;
//                bStream.position = 0;
//                bStream.isDispose = false;
//                bStream.referenceCount++;
//                return bStream;
//            }
//        }

//        public static void Push(BufferStream stream)
//        {
//            lock (SyncRoot)
//            {
//                if (stream.isDispose)
//                    return;
//                stream.isDispose = true;
//                Stack.Push(stream);
//            }
//        }

//        public static void Write(long seek, byte[] buffer, int index, int count)
//        {
//            lock (SyncRoot)
//            {
//                STREAM.Seek(seek, SeekOrigin.Begin);
//                STREAM.Write(buffer, index, count);
//            }
//        }

//        public static int Read(long seek, byte[] buffer, int index, int count)
//        {
//            lock (SyncRoot)
//            {
//                STREAM.Seek(seek, SeekOrigin.Begin);
//                return STREAM.Read(buffer, index, count);
//            }
//        }
//    }
//}