using Net.System;
using System;
using System.Reflection;

namespace Net.Share
{
    [Serializable]
    public class SyncVarInfo
    {
        internal ushort id;
        internal bool authorize;
        internal MethodInfo onValueChanged;
        internal bool isDispose;
        internal uint tick;

        public virtual void SetTarget(object target) { }
        public virtual void SetDefaultValue() { }
        public virtual void CheckHandlerValue(ref Segment segment, bool isWrite)
        {
        }
        public virtual SyncVarInfo Clone(object target)
        {
            return null;
        }
        public virtual bool EqualsTarget(object target)
        {
            return false;
        }
    }
    public delegate void SyncVarInfoDelegate<T, V>(T t, ref V v, ushort id, ref Segment segment, bool isWrite, Action<V, V> onValueChanged);
    public class SyncVarInfoPtr<T, V> : SyncVarInfo
    {
        internal T target;
        internal V value;
        internal SyncVarInfoDelegate<T, V> action;
        internal Action<V, V> action1;

        public SyncVarInfoPtr(SyncVarInfoDelegate<T, V> action)
        {
            this.action = action;
        }

        public override SyncVarInfo Clone(object target)
        {
            Action<V, V> action2 = null;
            if (onValueChanged != null)
                action2 = (Action<V, V>)onValueChanged.CreateDelegate(typeof(Action<V, V>), target);
            var segment = BufferPool.Take();
            V value1 = default;
            action((T)target, ref value1, id, ref segment, true, action2);
            segment.Dispose();
            return new SyncVarInfoPtr<T, V>(action) 
            {
                target = (T)target, id = id, authorize = authorize, action1 = action2, value = value1
            };
        }

        public override void SetTarget(object target)
        {
            this.target = (T)target;
        }

        public override void SetDefaultValue()
        {
            value = default;
        }

        public override void CheckHandlerValue(ref Segment segment, bool isWrite)
        {
            action(target, ref value, id, ref segment, isWrite, action1);
        }

        public override bool EqualsTarget(object target)
        {
            return this.target.Equals(target);
        }
    }
}