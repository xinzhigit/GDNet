using Net.Event;
using System;
using System.Threading;

namespace Net.System
{
    /// <summary>
    /// 主线程管理中心
    /// </summary>
    public static class ThreadManager
    {
        private static Thread MainThread;
        public static TimerEvent Event { get; private set; } = new TimerEvent();
        /// <summary>
        /// 时间计数间隔
        /// </summary>
        public static uint Interval { get; set; } = 1;
        /// <summary>
        /// 运行中?
        /// </summary>
        public static bool IsRuning { get; set; }

        
        static ThreadManager()
        {
            Init();
            Start();
        }

        private static void Init()
        {
            IsRuning = true;
        }

#if UNITY_EDITOR
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static async void OnInUnityEditor()
        {
            IsRuning = true;
            while (UnityEditor.EditorApplication.isPlaying)
                await global::System.Threading.Tasks.Task.Yield();
            IsRuning = false;
        }
#endif

        private static void Start()
        {
            MainThread = new Thread(Execute);
            MainThread.Name = "网络主线程";
            MainThread.IsBackground = true;
            MainThread.Start();
        }

        /// <summary>
        /// 控制台死循环线程
        /// </summary>
        public static void Run()
        {
            Stop();
            Execute();
        }

        /// <summary>
        /// unity update每帧调用
        /// </summary>
        public static void Run(uint interval)
        {
            Stop();
            Event.UpdateEventFixed(interval, false);
        }

        public static void Stop()
        {
            if (MainThread != null)
            {
                MainThread.Abort();
                MainThread = null;
            }
        }

        public static void Execute()
        {
            while (IsRuning)
            {
                try
                {
                    Event.UpdateEventFixed(Interval, true);
                }
                catch (ThreadAbortException ex)
                {
                    NDebug.LogWarning("主线程:" + ex.Message);
                }
                catch (Exception ex)
                {
                    NDebug.LogError("主线程异常:" + ex);
                }
            }
        }

        public static int Invoke(Func<bool> ptr, bool isAsync = false) 
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(Action ptr, bool isAsync = false)
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(string name, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, 0, ptr, isAsync);
        }

        public static int Invoke(float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(time, ptr, isAsync);
        }

        public static int Invoke(string name, float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, time, ptr, isAsync);
        }

        public static int Invoke(string name, int time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, time, ptr, isAsync);
        }
    }
}
