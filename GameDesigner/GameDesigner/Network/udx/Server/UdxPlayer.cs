using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Net.Server
{
    /// <summary>
    /// udx客户端对象
    /// </summary>
    public class UdxPlayer : NetPlayer
    {
        public IntPtr Udx { get; set; }
        public GCHandle Handle;

        public override void Dispose()
        {
            if (isDispose)
                return;
            base.Dispose();
            if (Handle.IsAllocated)
                Handle.Free();
        }
    }
}
