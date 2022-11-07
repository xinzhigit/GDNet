using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace Net.System
{
    internal sealed class System_StackDebugView<T>
    {
        private GStack<T> stack;

        public System_StackDebugView(GStack<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            this.stack = stack;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return stack.ToArray();
            }
        }
    }

    [DebuggerTypeProxy(typeof(System_StackDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
#if !SILVERLIGHT
    [Serializable()]
#endif
    [ComVisible(false)]
    public class GStack<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T>
    {
        private T[] _array;     // Storage for GStack elements
        private int _size;           // Number of items in the GStack.
        private int _version;        // Used to keep enumerator in sync w/ collection.
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private Object _syncRoot;

        private const int _defaultCapacity = 4;
        static T[] _emptyArray = new T[0];

        public GStack()
        {
            _array = _emptyArray;
            _size = 0;
            _version = 0;
        }

        public GStack(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            _array = new T[capacity];
            _size = 0;
            _version = 0;
        }

        public GStack(IEnumerable<T> collection)
        {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                _array = new T[count];
                c.CopyTo(_array, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                _array = new T[_defaultCapacity];

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Push(en.Current);
                    }
                }
            }
        }

        public int Count
        {
            get { return _size; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        Object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            _size = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            int count = _size;

            EqualityComparer<T> c = EqualityComparer<T>.Default;
            while (count-- > 0)
            {
                if (((Object)item) == null)
                {
                    if (((Object)_array[count]) == null)
                        return true;
                }
                else if (_array[count] != null && c.Equals(_array[count], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (array.Length - arrayIndex < _size)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            Array.Copy(_array, 0, array, arrayIndex, _size);
            Array.Reverse(array, arrayIndex, _size);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }

            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (array.Length - arrayIndex < _size)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            try
            {
                Array.Copy(_array, 0, array, arrayIndex, _size);
                Array.Reverse(array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void TrimExcess()
        {
            int threshold = (int)(((double)_array.Length) * 0.9);
            if (_size < threshold)
            {
                T[] newarray = new T[_size];
                Array.Copy(_array, 0, newarray, 0, _size);
                _array = newarray;
                _version++;
            }
        }

        public T Peek()
        {
            if (_size == 0)
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
            return _array[_size - 1];
        }

        public T Pop()
        {
            if (_size == 0)
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
            _version++;
            T item = _array[--_size];
            _array[_size] = default(T);     // Free memory quicker.
            return item;
        }

        public bool TryPop(out T item) 
        {
            if (Count > 0)
            {
                item = Pop();
                return true;
            }
            item = default;
            return false;
        }

        public void Push(T item)
        {
            if (_size == _array.Length)
            {
                T[] newArray = new T[(_array.Length == 0) ? _defaultCapacity : 2 * _array.Length];
                Array.Copy(_array, 0, newArray, 0, _size);
                _array = newArray;
            }
            _array[_size++] = item;
            _version++;
        }

        public T[] ToArray()
        {
            T[] objArray = new T[_size];
            int i = 0;
            while (i < _size)
            {
                objArray[i] = _array[_size - i - 1];
                i++;
            }
            return objArray;
        }

#if !SILVERLIGHT
        [Serializable()]
#endif
        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private GStack<T> _stack;
            private int _index;
            private int _version;
            private T currentElement;

            internal Enumerator(GStack<T> stack)
            {
                _stack = stack;
                _version = _stack._version;
                _index = -2;
                currentElement = default(T);
            }

            public void Dispose()
            {
                _index = -1;
            }

            public bool MoveNext()
            {
                bool retval;
                if (_index == -2)
                {
                    _index = _stack._size - 1;
                    retval = (_index >= 0);
                    if (retval)
                        currentElement = _stack._array[_index];
                    return retval;
                }
                if (_index == -1)
                {
                    return false;
                }

                retval = (--_index >= 0);
                if (retval)
                    currentElement = _stack._array[_index];
                else
                    currentElement = default(T);
                return retval;
            }

            public T Current
            {
                get
                {
                    return currentElement;
                }
            }

            Object IEnumerator.Current
            {
                get
                {
                    return currentElement;
                }
            }

            void IEnumerator.Reset()
            {
                _index = -2;
                currentElement = default(T);
            }
        }
    }
}