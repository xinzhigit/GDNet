namespace Net.Share
{
    using global::System;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;

    /// <summary>
    /// 提供win32内核api类
    /// </summary>
    public static class Win32KernelAPI
    {
        /// <summary>
        /// <code>设置应用程序或驱动程序使用的最小定时器分辨率</code>
        /// 系统时钟间隔是个很少被关心到的系统标量，
        /// 它反映了系统产生时钟中断的频率，间隔越小频率越高，反之亦然。
        /// 每当时钟中断产生，系统相关的中断函数将会处理这个中断。
        /// 时钟中断处理函数会更新系统时间，检查内核调试信息等。
        /// </summary>
        /// <param name="uMilliseconds"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeBeginPeriod(uint uMilliseconds);

        /// <summary>
        /// 清除应用程序或驱动程序使用的最小定时器分辨率  
        /// </summary>
        /// <param name="uMilliseconds"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern uint timeEndPeriod(uint uMilliseconds);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public unsafe static extern int sendto([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags, [In] byte[] socketAddress, [In] int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public unsafe static extern int send([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags);

        [DllImport("ws2_32.dll", SetLastError = true)]
        internal static extern int select([In] int ignoredParameter, [In][Out] IntPtr[] readfds, [In][Out] IntPtr[] writefds, [In][Out] IntPtr[] exceptfds, [In] IntPtr nullTimeout);

        [DllImport("ws2_32.dll", SetLastError = true)]
        internal static extern int select([In] int ignoredParameter, [In][Out] IntPtr[] readfds, [In][Out] IntPtr[] writefds, [In][Out] IntPtr[] exceptfds, [In] ref TimeValue timeout);

        [DllImport("ws2_32.dll", SetLastError = true)]
        internal unsafe static extern int recv([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] int len, [In] SocketFlags socketFlags);

        [DllImport("Kernel32.dll")]
        private static extern Boolean SetSystemTime([In, Out] SystemTime st);

        [DllImport("ws2_32.dll", SetLastError = true)]
        internal static extern SocketError WSASendTo([In] IntPtr socketHandle, [In] ref WSABuffer buffer, [In] int bufferCount, out int bytesTransferred, [In] SocketFlags socketFlags, [In] byte[] socketAddress, [In] int socketAddressSize, [In] SafeHandle overlapped, [In] IntPtr completionRoutine);

        /// <summary>
        /// 设置系统时间
        /// </summary>
        /// <param name="newdatetime">新时间</param>
        /// <returns></returns>
        public static bool SetSysTime(DateTime newdatetime)
        {
            SystemTime st = new SystemTime();
            st.year = Convert.ToUInt16(newdatetime.Year);
            st.month = Convert.ToUInt16(newdatetime.Month);
            st.day = Convert.ToUInt16(newdatetime.Day);
            st.dayofweek = Convert.ToUInt16(newdatetime.DayOfWeek);
            st.hour = Convert.ToUInt16(newdatetime.Hour - TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(2001, 09, 01)).Hours);
            st.minute = Convert.ToUInt16(newdatetime.Minute);
            st.second = Convert.ToUInt16(newdatetime.Second);
            st.milliseconds = Convert.ToUInt16(newdatetime.Millisecond);
            return SetSystemTime(st);
        }
    }

    public struct TimeValue
    {
        public int Seconds;
        public int Microseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SystemTime
    {
        public ushort year;
        public ushort month;
        public ushort dayofweek;
        public ushort day;
        public ushort hour;
        public ushort minute;
        public ushort second;
        public ushort milliseconds;
    }

    internal struct WSABuffer
    {
        internal int Length;
        internal IntPtr Pointer;
    }

    internal class SafeNativeOverlapped : SafeHandle
    {
        internal SafeNativeOverlapped() : this(IntPtr.Zero)
        {
        }
        internal unsafe SafeNativeOverlapped(NativeOverlapped* handle) : this((IntPtr)handle)
        {
        }
        internal SafeNativeOverlapped(IntPtr handle) : base(IntPtr.Zero, true)
        {
            SetHandle(handle);
        }
        public override bool IsInvalid => handle == IntPtr.Zero;
        protected unsafe override bool ReleaseHandle()
        {
            IntPtr intPtr = Interlocked.Exchange(ref handle, IntPtr.Zero);
            if (intPtr != IntPtr.Zero)
            {
                Overlapped.Free((NativeOverlapped*)(void*)intPtr);
            }
            return true;
        }
    }
}
