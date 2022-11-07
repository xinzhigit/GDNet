using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Share
{
    public class ThreadGroup
    {
        public int Id;
        public Thread Thread;

        public override string ToString()
        {
            return $"线程组ID:{Id} 线程ID:{Thread.ManagedThreadId}";
        }
    }
}
