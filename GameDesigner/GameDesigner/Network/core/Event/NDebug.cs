namespace Net.Event
{
    using global::System;
#if SERVICE
    using global::System.Drawing;
    using global::System.Windows.Forms;
#endif
    using Net.System;

    public enum LogType
    {
        Log,
        Warning,
        Error,
    }

    public class LogEntity
    {
        public int count;
        public int row = -1;
        public DateTime time;
        public LogType log;
        public string msg;

        public override string ToString()
        {
            return $"[{time.ToString("yyyy-MM-dd HH:mm:ss:ms")}][{log}] {msg}";
        }
    }

    /// <summary>
    /// 控制台输出帮助类
    /// </summary>
    public class ConsoleDebug : IDebug
    {
        private MyDictionary<string, LogEntity> dic = new MyDictionary<string, LogEntity>();
        public int count = 1000;
        private int cursorTop;

        public void Output(DateTime time, LogType log, string msg)
        {
            if (dic.Count > count)
            {
                dic.Clear();
                Console.Clear();
                cursorTop = 0;
            }
            if (!dic.TryGetValue(log + msg, out var entity))
                dic.TryAdd(log + msg, entity = new LogEntity() { time = time, log = log, msg = msg });
            entity.count++;
            if (entity.row == -1)
            {
                entity.row = cursorTop;
                Console.SetCursorPosition(0, cursorTop);
            }
            else
            {
                Console.SetCursorPosition(0, entity.row);
            }
            var info = $"[{time.ToString("yyyy-MM-dd HH:mm:ss:ms")}][";
            Console.Write(info);
            Console.ForegroundColor = log == LogType.Log ? ConsoleColor.Green : log == LogType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red;
            info = $"{log}";
            Console.Write(info);
            Console.ResetColor();
            if (entity.count > 1)
                Console.Write($"] ({entity.count}) {msg}\r\n");
            else
                Console.Write($"] {msg}\r\n");
            if(Console.CursorTop > cursorTop)
                cursorTop = Console.CursorTop;
        }
    }

#if SERVICE
    /// <summary>
    /// Form窗口程序输出帮助类
    /// </summary>
    public class FormDebug : IDebug
    {
        private MyDictionary<string, LogEntity> dic = new MyDictionary<string, LogEntity>();
        public int count = 1000;
        public ListBox listBox;

        public FormDebug(ListBox listBox) 
        {
            this.listBox = listBox;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.DrawItem += DrawItem;
        }

        public void Output(DateTime time, LogType log, string msg)
        {
            if (dic.Count > count)
            {
                dic.Clear();
                Console.Clear();
            }
            if (!dic.TryGetValue(log + msg, out var entity))
                dic.TryAdd(log + msg, entity = new LogEntity() { time = time, log = log, msg = msg });
            entity.count++;
            if (entity.row == -1)
            {
                entity.row = listBox.Items.Count;
                listBox.Items.Add(entity);
            }
            //listBox.Refresh();
        }

        public void DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            var entity = listBox.Items[e.Index] as LogEntity;
            e.DrawBackground();
            e.DrawFocusRectangle();
            var y = e.Bounds.Y;
            var msg = $"[{entity.time.ToString("yyyy-MM-dd HH:mm:ss:ms")}][";
            e.Graphics.DrawString(msg, e.Font, Brushes.Black, 0, y);
            var x = msg.Length * 6;
            msg = $"{entity.log}";
            var color = entity.log == LogType.Log ? Brushes.Blue : entity.log == LogType.Warning ? Brushes.Yellow : Brushes.Red;
            e.Graphics.DrawString(msg, e.Font, color, x, y);
            x += msg.Length * 6;
            msg = entity.msg.Split('\r', '\n')[0];
            if (entity.count > 1)
                e.Graphics.DrawString($"] ({entity.count}) {msg}", e.Font, Brushes.Black, x, y);
            else
                e.Graphics.DrawString($"] {msg}", e.Font, Brushes.Black, x, y);
        }
    }
#endif

    public interface IDebug 
    {
        void Output(DateTime time, LogType log, string msg);
    }

    /// <summary>
    /// 消息输入输出处理类
    /// </summary>
    public static class NDebug
    {
        /// <summary>
        /// 输出调式消息
        /// </summary>
        public static event Action<string> LogHandle;
        /// <summary>
        /// 输出调式错误消息
        /// </summary>
        public static event Action<string> LogErrorHandle;
        /// <summary>
        /// 输出调式警告消息
        /// </summary>
        public static event Action<string> LogWarningHandle;
        /// <summary>
        /// 输出信息处理事件
        /// </summary>
        public static event Action<DateTime, LogType, string> Output;
        /// <summary>
        /// 输出日志最多容纳条数
        /// </summary>
        public static int LogMax { get; set; } = 500;
        /// <summary>
        /// 输出错误日志最多容纳条数
        /// </summary>
        public static int LogErrorMax { get; set; } = 500;
        /// <summary>
        /// 输出警告日志最多容纳条数
        /// </summary>
        public static int LogWarningMax { get; set; } = 500;

        private static QueueSafe<object> logQueue = new QueueSafe<object>();
        private static QueueSafe<object> errorQueue = new QueueSafe<object>();
        private static QueueSafe<object> warningQueue = new QueueSafe<object>();

        private static IDebug debug;

#if SERVICE
        static NDebug()
        {
            Handler();
        }

        private static void Handler()
        {
            ThreadManager.Invoke("Debug-Log", ()=> 
            {
                try
                {
                    if (logQueue.TryDequeue(out object message))
                    {
                        LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
                        Output?.Invoke(DateTime.Now, LogType.Log, message.ToString());
                    }
                    if (warningQueue.TryDequeue(out message))
                    {
                        LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
                        Output?.Invoke(DateTime.Now, LogType.Warning, message.ToString());
                    }
                    if (errorQueue.TryDequeue(out message))
                    {
                        LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
                        Output?.Invoke(DateTime.Now, LogType.Error, message.ToString());
                    }
                    if (logQueue.Count >= LogMax)
                        logQueue = new QueueSafe<object>();
                    if (errorQueue.Count >= LogErrorMax)
                        errorQueue = new QueueSafe<object>();
                    if (warningQueue.Count >= LogWarningMax)
                        warningQueue = new QueueSafe<object>();
                }
                catch (Exception ex)
                {
                    errorQueue.Enqueue(ex.Message);
                }
                return true;
            }, true);
        }
#endif

        /// <summary>
        /// 输出调式消息
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
#if SERVICE
            logQueue.Enqueue(message);
#else
            LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
            Output?.Invoke(DateTime.Now, LogType.Log, message.ToString());
#endif
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
#if SERVICE
            errorQueue.Enqueue(message);
#else
            LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
            Output?.Invoke(DateTime.Now, LogType.Error, message.ToString());
#endif
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
#if SERVICE
            warningQueue.Enqueue(message);
#else
            LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
            Output?.Invoke(DateTime.Now, LogType.Warning, message.ToString());
#endif
        }

        public static void BindLogAll(Action<string> log)
        {
            BindLogAll(log, log, log);
        }

        public static void BindLogAll(Action<string> log, Action<string> warning, Action<string> error)
        {
            if (log != null) LogHandle += log;
            if (warning != null) LogWarningHandle += warning;
            if (error != null) LogErrorHandle += error;
        }

        public static void RemoveLogAll(Action<string> log)
        {
            RemoveLogAll(log, log, log);
        }

        public static void RemoveLogAll(Action<string> log, Action<string> warning, Action<string> error)
        {
            if (log != null) LogHandle -= log;
            if (warning != null) LogWarningHandle -= warning;
            if (error != null) LogErrorHandle -= error;
        }

        /// <summary>
        /// 绑定控制台输出
        /// </summary>
        public static void BindConsoleLog()
        {
            debug = new ConsoleDebug();
            NDebug.Output += debug.Output;
        }

        /// <summary>
        /// 移除控制台输出
        /// </summary>
        public static void RemoveConsoleLog()
        {
            RemoveDebug();
        }

        /// <summary>
        /// 绑定输出接口
        /// </summary>
        /// <param name="log"></param>
        public static void BindDebug(IDebug log)
        {
            debug = log;
            NDebug.Output += debug.Output;
        }

        /// <summary>
        /// 移除输出接口
        /// </summary>
        public static void RemoveDebug()
        {
            NDebug.Output -= debug.Output;
        }
    }
}