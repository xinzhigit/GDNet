using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Share
{
    /// <summary>
    /// 当运行时调用，如果在unity会自动转到untiy的RuntimeInitializeOnLoadMethod特性
    /// </summary>
    public class RuntimeInitializeOnLoadMethod : Attribute
    {
    }
}
