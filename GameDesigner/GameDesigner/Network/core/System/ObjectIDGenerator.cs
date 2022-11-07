using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Net.System
{
    public class ObjectIDGenerator
    {
        private long m_currentCount;
        public MyDictionary<object, long> dict = new MyDictionary<object, long>();

        public virtual long GetId(object obj)
        {
            if (!dict.TryGetValue(obj, out var id))
                dict.Add(obj, id = m_currentCount++);
            return id;
        }

        public bool TryRemove(object obj, out long id) => dict.TryRemove(obj, out id);
    }
}
