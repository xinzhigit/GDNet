using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
#if !SILVERLIGHT
using System.Runtime.Serialization;
#endif
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;
#if SILVERLIGHT
using System.Core; // for System.Core.SR
#endif

namespace Net.System
{
    unsafe internal class BitHelper
    {   // should not be serialized

        private const byte MarkedBitFlag = 1;
        private const byte IntSize = 32;

        // m_length of underlying int array (not logical bit array)
        private int m_length;

        // ptr to stack alloc'd array of ints
        [SecurityCritical]
        private int* m_arrayPtr;

        // array of ints
        private int[] m_array;

        // whether to operate on stack alloc'd or heap alloc'd array 
        private bool useStackAlloc;

        /// <summary>
        /// Instantiates a BitHelper with a heap alloc'd array of ints
        /// </summary>
        /// <param name="bitArray">int array to hold bits</param>
        /// <param name="length">length of int array</param>
        [SecurityCritical]
        internal BitHelper(int* bitArrayPtr, int length)
        {
            this.m_arrayPtr = bitArrayPtr;
            this.m_length = length;
            useStackAlloc = true;
        }

        /// <summary>
        /// Instantiates a BitHelper with a heap alloc'd array of ints
        /// </summary>
        /// <param name="bitArray">int array to hold bits</param>
        /// <param name="length">length of int array</param>
        internal BitHelper(int[] bitArray, int length)
        {
            this.m_array = bitArray;
            this.m_length = length;
        }

        /// <summary>
        /// Mark bit at specified position
        /// </summary>
        /// <param name="bitPosition"></param>
        [SecuritySafeCritical]
        internal unsafe void MarkBit(int bitPosition)
        {
            if (useStackAlloc)
            {
                int bitArrayIndex = bitPosition / IntSize;
                if (bitArrayIndex < m_length && bitArrayIndex >= 0)
                {
                    m_arrayPtr[bitArrayIndex] |= (MarkedBitFlag << (bitPosition % IntSize));
                }
            }
            else
            {
                int bitArrayIndex = bitPosition / IntSize;
                if (bitArrayIndex < m_length && bitArrayIndex >= 0)
                {
                    m_array[bitArrayIndex] |= (MarkedBitFlag << (bitPosition % IntSize));
                }
            }
        }

        /// <summary>
        /// Is bit at specified position marked?
        /// </summary>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        internal unsafe bool IsMarked(int bitPosition)
        {
            if (useStackAlloc)
            {
                int bitArrayIndex = bitPosition / IntSize;
                if (bitArrayIndex < m_length && bitArrayIndex >= 0)
                {
                    return ((m_arrayPtr[bitArrayIndex] & (MarkedBitFlag << (bitPosition % IntSize))) != 0);
                }
                return false;
            }
            else
            {
                int bitArrayIndex = bitPosition / IntSize;
                if (bitArrayIndex < m_length && bitArrayIndex >= 0)
                {
                    return ((m_array[bitArrayIndex] & (MarkedBitFlag << (bitPosition % IntSize))) != 0);
                }
                return false;
            }
        }

        /// <summary>
        /// How many ints must be allocated to represent n bits. Returns (n+31)/32, but 
        /// avoids overflow
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        internal static int ToIntArrayLength(int n)
        {
            return n > 0 ? ((n - 1) / IntSize + 1) : 0;
        }

    }

    internal class HashSetSafeDebugView<T>
    {
        private HashSetSafe<T> set;

        public HashSetSafeDebugView(HashSetSafe<T> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
            }

            this.set = set;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return set.ToArray();
            }
        }
    }

    [DebuggerTypeProxy(typeof(HashSetSafeDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By design")]
#if SILVERLIGHT
    public class HashSet<T> : ICollection<T>, ISet<T>, IReadOnlyCollection<T>
#else
    [Serializable()]
    [HostProtection(MayLeakOnAbort = true)]
    public class HashSetSafe<T> : ICollection<T>, ISerializable, IDeserializationCallback, ISet<T>, IReadOnlyCollection<T>
#endif
    {
        private const int Lower31BitMask = 0x7FFFFFFF;
        private const int StackAllocThreshold = 100;
        private const int ShrinkThreshold = 3;

#if !SILVERLIGHT
        private const String CapacityName = "Capacity";
        private const String ElementsName = "Elements";
        private const String ComparerName = "Comparer";
        private const String VersionName = "Version";
#endif

        private int[] m_buckets;
        private Slot[] m_slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private IEqualityComparer<T> m_comparer;
        private int m_version;
        private object syncRoot = new object();

#if !SILVERLIGHT
        private SerializationInfo m_siInfo;
#endif

        #region Constructors

        public HashSetSafe()
            : this(EqualityComparer<T>.Default) { }

        public HashSetSafe(int capacity)
            : this(capacity, EqualityComparer<T>.Default) { }

        public HashSetSafe(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            this.m_comparer = comparer;
            m_lastIndex = 0;
            m_count = 0;
            m_freeList = -1;
            m_version = 0;
        }

        public HashSetSafe(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default) { }

        public HashSetSafe(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            Contract.EndContractBlock();

            var otherAsHashSet = collection as HashSetSafe<T>;
            if (otherAsHashSet != null && AreEqualityComparersEqual(this, otherAsHashSet))
            {
                CopyFrom(otherAsHashSet);
            }
            else
            {
                ICollection<T> coll = collection as ICollection<T>;
                int suggestedCapacity = coll == null ? 0 : coll.Count;
                Initialize(suggestedCapacity);

                this.UnionWith(collection);

                if (m_count > 0 && m_slots.Length / m_count > ShrinkThreshold)
                {
                    TrimExcess();
                }
            }
        }

        private void CopyFrom(HashSetSafe<T> source)
        {
            int count = source.m_count;
            if (count == 0)
            {
                return;
            }

            int capacity = source.m_buckets.Length;
            int threshold = HashHelpers.ExpandPrime(count + 1);

            if (threshold >= capacity)
            {
                m_buckets = (int[])source.m_buckets.Clone();
                m_slots = (Slot[])source.m_slots.Clone();

                m_lastIndex = source.m_lastIndex;
                m_freeList = source.m_freeList;
            }
            else
            {
                int lastIndex = source.m_lastIndex;
                Slot[] slots = source.m_slots;
                Initialize(count);
                int index = 0;
                for (int i = 0; i < lastIndex; ++i)
                {
                    int hashCode = slots[i].hashCode;
                    if (hashCode >= 0)
                    {
                        AddValue(index, hashCode, slots[i].value);
                        ++index;
                    }
                }
                Debug.Assert(index == count);
                m_lastIndex = index;
            }
            m_count = count;
        }

#if !SILVERLIGHT
        protected HashSetSafe(SerializationInfo info, StreamingContext context)
        {
            m_siInfo = info;
        }
#endif

        public HashSetSafe(int capacity, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
            Contract.EndContractBlock();

            if (capacity > 0)
            {
                Initialize(capacity);
            }
        }

        #endregion

        #region ICollection<T> methods

        void ICollection<T>.Add(T item)
        {
            AddIfNotPresent(item);
        }

        public void Clear()
        {
            if (m_lastIndex > 0)
            {
                Debug.Assert(m_buckets != null, "m_buckets was null but m_lastIndex > 0");

                Array.Clear(m_slots, 0, m_lastIndex);
                Array.Clear(m_buckets, 0, m_buckets.Length);
                m_lastIndex = 0;
                m_count = 0;
                m_freeList = -1;
            }
            m_version++;
        }

        public bool Contains(T item)
        {
            if (m_buckets != null)
            {
                int hashCode = InternalGetHashCode(item);
                for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next)
                {
                    if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, m_count);
        }

        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                if (m_buckets != null)
                {
                    int hashCode = InternalGetHashCode(item);
                    int bucket = hashCode % m_buckets.Length;
                    int last = -1;
                    for (int i = m_buckets[bucket] - 1; i >= 0; last = i, i = m_slots[i].next)
                    {
                        if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, item))
                        {
                            if (last < 0)
                            {
                                m_buckets[bucket] = m_slots[i].next + 1;
                            }
                            else
                            {
                                m_slots[last].next = m_slots[i].next;
                            }
                            m_slots[i].hashCode = -1;
                            m_slots[i].value = default(T);
                            m_slots[i].next = m_freeList;

                            m_count--;
                            m_version++;
                            if (m_count == 0)
                            {
                                m_lastIndex = 0;
                                m_freeList = -1;
                            }
                            else
                            {
                                m_freeList = i;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public int Count
        {
            get { return m_count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable methods

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

        #endregion

        #region ISerializable methods

#if !SILVERLIGHT
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue(VersionName, m_version);

#if FEATURE_RANDOMIZED_STRING_HASHING && !FEATURE_NETCORE
            info.AddValue(ComparerName, HashHelpers.GetEqualityComparerForSerialization(m_comparer), typeof(IEqualityComparer<T>));
#else
            info.AddValue(ComparerName, m_comparer, typeof(IEqualityComparer<T>));
#endif

            info.AddValue(CapacityName, m_buckets == null ? 0 : m_buckets.Length);
            if (m_buckets != null)
            {
                T[] array = new T[m_count];
                CopyTo(array);
                info.AddValue(ElementsName, array, typeof(T[]));
            }
        }
#endif
        #endregion

        #region IDeserializationCallback methods

#if !SILVERLIGHT
        public virtual void OnDeserialization(Object sender)
        {

            if (m_siInfo == null)
            {
                return;
            }

            int capacity = m_siInfo.GetInt32(CapacityName);
            m_comparer = (IEqualityComparer<T>)m_siInfo.GetValue(ComparerName, typeof(IEqualityComparer<T>));
            m_freeList = -1;

            if (capacity != 0)
            {
                m_buckets = new int[capacity];
                m_slots = new Slot[capacity];

                T[] array = (T[])m_siInfo.GetValue(ElementsName, typeof(T[]));

                if (array == null)
                {
                    throw new SerializationException();
                }

                for (int i = 0; i < array.Length; i++)
                {
                    AddIfNotPresent(array[i]);
                }
            }
            else
            {
                m_buckets = null;
            }

            m_version = m_siInfo.GetInt32(VersionName);
            m_siInfo = null;
        }
#endif

        #endregion

        #region HashSet methods

        public bool Add(T item)
        {
            lock (syncRoot)
            {
                return AddIfNotPresent(item);
            }
        }

        public bool TryGetValue(T equalValue, out T actualValue)
        {
            if (m_buckets != null)
            {
                int i = InternalIndexOf(equalValue);
                if (i >= 0)
                {
                    actualValue = m_slots[i].value;
                    return true;
                }
            }
            actualValue = default(T);
            return false;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            foreach (T item in other)
            {
                AddIfNotPresent(item);
            }
        }

        
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                return;
            }

            ICollection<T> otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                if (otherAsCollection.Count == 0)
                {
                    Clear();
                    return;
                }

                HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
                if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
                {
                    IntersectWithHashSetWithSameEC(otherAsSet);
                    return;
                }
            }

            IntersectWithEnumerable(other);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                return;
            }

            if (other == this)
            {
                Clear();
                return;
            }

            foreach (T element in other)
            {
                Remove(element);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                UnionWith(other);
                return;
            }

            if (other == this)
            {
                Clear();
                return;
            }

            HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
            if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
            {
                SymmetricExceptWithUniqueHashSet(otherAsSet);
            }
            else
            {
                SymmetricExceptWithEnumerable(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                return true;
            }

            HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
            if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
            {
                if (m_count > otherAsSet.Count)
                {
                    return false;
                }

                return IsSubsetOfHashSetWithSameEC(otherAsSet);
            }
            else
            {
                ElementCount result = CheckUniqueAndUnfoundElements(other, false);
                return (result.uniqueCount == m_count && result.unfoundCount >= 0);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            ICollection<T> otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                if (m_count == 0)
                {
                    return otherAsCollection.Count > 0;
                }
                HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
                if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
                {
                    if (m_count >= otherAsSet.Count)
                    {
                        return false;
                    }
                    return IsSubsetOfHashSetWithSameEC(otherAsSet);
                }
            }

            ElementCount result = CheckUniqueAndUnfoundElements(other, false);
            return (result.uniqueCount == m_count && result.unfoundCount > 0);

        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            ICollection<T> otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                if (otherAsCollection.Count == 0)
                {
                    return true;
                }
                HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
                if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
                {
                    if (otherAsSet.Count > m_count)
                    {
                        return false;
                    }
                }
            }

            return ContainsAllElements(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                return false;
            }

            ICollection<T> otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                if (otherAsCollection.Count == 0)
                {
                    return true;
                }
                HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
                if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
                {
                    if (otherAsSet.Count >= m_count)
                    {
                        return false;
                    }
                    return ContainsAllElements(otherAsSet);
                }
            }
            ElementCount result = CheckUniqueAndUnfoundElements(other, true);
            return (result.uniqueCount < m_count && result.unfoundCount == 0);

        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            if (m_count == 0)
            {
                return false;
            }

            foreach (T element in other)
            {
                if (Contains(element))
                {
                    return true;
                }
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            Contract.EndContractBlock();

            HashSetSafe<T> otherAsSet = other as HashSetSafe<T>;
            if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet))
            {
                if (m_count != otherAsSet.Count)
                {
                    return false;
                }
                return ContainsAllElements(otherAsSet);
            }
            else
            {
                ICollection<T> otherAsCollection = other as ICollection<T>;
                if (otherAsCollection != null)
                {
                    if (m_count == 0 && otherAsCollection.Count > 0)
                    {
                        return false;
                    }
                }
                ElementCount result = CheckUniqueAndUnfoundElements(other, true);
                return (result.uniqueCount == m_count && result.unfoundCount == 0);
            }
        }

        public void CopyTo(T[] array) { CopyTo(array, 0, m_count); }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            Contract.EndContractBlock();

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "");
            }

            if (arrayIndex > array.Length || count > array.Length - arrayIndex)
            {
                throw new ArgumentException("");
            }

            int numCopied = 0;
            for (int i = 0; i < m_lastIndex && numCopied < count; i++)
            {
                if (m_slots[i].hashCode >= 0)
                {
                    array[arrayIndex + numCopied] = m_slots[i].value;
                    numCopied++;
                }
            }
        }

        public int RemoveWhere(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            Contract.EndContractBlock();

            int numRemoved = 0;
            for (int i = 0; i < m_lastIndex; i++)
            {
                if (m_slots[i].hashCode >= 0)
                {
                    T value = m_slots[i].value;
                    if (match(value))
                    {
                        if (Remove(value))
                        {
                            numRemoved++;
                        }
                    }
                }
            }
            return numRemoved;
        }

        
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return m_comparer;
            }
        }

        
        public void TrimExcess()
        {
            Debug.Assert(m_count >= 0, "m_count is negative");

            if (m_count == 0)
            {
                m_buckets = null;
                m_slots = null;
                m_version++;
            }
            else
            {
                Debug.Assert(m_buckets != null, "m_buckets was null but m_count > 0");

                
                int newSize = HashHelpers.GetPrime(m_count);
                Slot[] newSlots = new Slot[newSize];
                int[] newBuckets = new int[newSize];

                int newIndex = 0;
                for (int i = 0; i < m_lastIndex; i++)
                {
                    if (m_slots[i].hashCode >= 0)
                    {
                        newSlots[newIndex] = m_slots[i];

                        int bucket = newSlots[newIndex].hashCode % newSize;
                        newSlots[newIndex].next = newBuckets[bucket] - 1;
                        newBuckets[bucket] = newIndex + 1;

                        newIndex++;
                    }
                }

                Debug.Assert(newSlots.Length <= m_slots.Length, "capacity increased after TrimExcess");

                m_lastIndex = newIndex;
                m_slots = newSlots;
                m_buckets = newBuckets;
                m_freeList = -1;
            }
        }

        #endregion

        #region Helper methods

        private void Initialize(int capacity)
        {
            Debug.Assert(m_buckets == null, "Initialize was called but m_buckets was non-null");

            int size = HashHelpers.GetPrime(capacity);

            m_buckets = new int[size];
            m_slots = new Slot[size];
        }

        private void IncreaseCapacity()
        {
            Debug.Assert(m_buckets != null, "IncreaseCapacity called on a set with no elements");

            int newSize = HashHelpers.ExpandPrime(m_count);
            if (newSize <= m_count)
            {
                throw new ArgumentException();
            }

            SetCapacity(newSize, false);
        }

        private void SetCapacity(int newSize, bool forceNewHashCodes)
        {
            Contract.Assert(HashHelpers.IsPrime(newSize), "New size is not prime!");

            Contract.Assert(m_buckets != null, "SetCapacity called on a set with no elements");

            Slot[] newSlots = new Slot[newSize];
            if (m_slots != null)
            {
                Array.Copy(m_slots, 0, newSlots, 0, m_lastIndex);
            }

            if (forceNewHashCodes)
            {
                for (int i = 0; i < m_lastIndex; i++)
                {
                    if (newSlots[i].hashCode != -1)
                    {
                        newSlots[i].hashCode = InternalGetHashCode(newSlots[i].value);
                    }
                }
            }

            int[] newBuckets = new int[newSize];
            for (int i = 0; i < m_lastIndex; i++)
            {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }
            m_slots = newSlots;
            m_buckets = newBuckets;
        }

        private bool AddIfNotPresent(T value)
        {
            if (m_buckets == null)
            {
                Initialize(0);
            }

            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % m_buckets.Length;
#if FEATURE_RANDOMIZED_STRING_HASHING && !FEATURE_NETCORE
            int collisionCount = 0;
#endif
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next)
            {
                if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, value))
                {
                    return false;
                }
#if FEATURE_RANDOMIZED_STRING_HASHING && !FEATURE_NETCORE
                collisionCount++;
#endif
            }

            int index;
            if (m_freeList >= 0)
            {
                index = m_freeList;
                m_freeList = m_slots[index].next;
            }
            else
            {
                if (m_lastIndex == m_slots.Length)
                {
                    IncreaseCapacity();
                    bucket = hashCode % m_buckets.Length;
                }
                index = m_lastIndex;
                m_lastIndex++;
            }
            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
            m_count++;
            m_version++;

#if FEATURE_RANDOMIZED_STRING_HASHING && !FEATURE_NETCORE
            if(collisionCount > HashHelpers.HashCollisionThreshold && HashHelpers.IsWellKnownEqualityComparer(m_comparer)) {
                m_comparer = (IEqualityComparer<T>) HashHelpers.GetRandomizedEqualityComparer(m_comparer);
                SetCapacity(m_buckets.Length, true);
            }
#endif

            return true;
        }

        private void AddValue(int index, int hashCode, T value)
        {
            int bucket = hashCode % m_buckets.Length;

#if DEBUG
            Debug.Assert(InternalGetHashCode(value) == hashCode);
            for (int i = m_buckets[bucket] - 1; i >= 0; i = m_slots[i].next)
            {
                Debug.Assert(!m_comparer.Equals(m_slots[i].value, value));
            }
#endif

            Debug.Assert(m_freeList == -1);
            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
        }

        private bool ContainsAllElements(IEnumerable<T> other)
        {
            foreach (T element in other)
            {
                if (!Contains(element))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsSubsetOfHashSetWithSameEC(HashSetSafe<T> other)
        {

            foreach (T item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        private void IntersectWithHashSetWithSameEC(HashSetSafe<T> other)
        {
            for (int i = 0; i < m_lastIndex; i++)
            {
                if (m_slots[i].hashCode >= 0)
                {
                    T item = m_slots[i].value;
                    if (!other.Contains(item))
                    {
                        Remove(item);
                    }
                }
            }
        }

        [SecuritySafeCritical]
        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            Debug.Assert(m_buckets != null, "m_buckets shouldn't be null; callers should check first");

            int originalLastIndex = m_lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            BitHelper bitHelper;
            if (intArrayLength <= StackAllocThreshold)
            {
                int* bitArrayPtr = stackalloc int[intArrayLength];
                bitHelper = new BitHelper(bitArrayPtr, intArrayLength);
            }
            else
            {
                int[] bitArray = new int[intArrayLength];
                bitHelper = new BitHelper(bitArray, intArrayLength);
            }

            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    bitHelper.MarkBit(index);
                }
            }

            for (int i = 0; i < originalLastIndex; i++)
            {
                if (m_slots[i].hashCode >= 0 && !bitHelper.IsMarked(i))
                {
                    Remove(m_slots[i].value);
                }
            }
        }

        private int InternalIndexOf(T item)
        {
            Debug.Assert(m_buckets != null, "m_buckets was null; callers should check first");

            int hashCode = InternalGetHashCode(item);
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next)
            {
                if ((m_slots[i].hashCode) == hashCode && m_comparer.Equals(m_slots[i].value, item))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SymmetricExceptWithUniqueHashSet(HashSetSafe<T> other)
        {
            foreach (T item in other)
            {
                if (!Remove(item))
                {
                    AddIfNotPresent(item);
                }
            }
        }

        [SecuritySafeCritical]
        private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other)
        {
            int originalLastIndex = m_lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            BitHelper itemsToRemove;
            BitHelper itemsAddedFromOther;
            if (intArrayLength <= StackAllocThreshold / 2)
            {
                int* itemsToRemovePtr = stackalloc int[intArrayLength];
                itemsToRemove = new BitHelper(itemsToRemovePtr, intArrayLength);

                int* itemsAddedFromOtherPtr = stackalloc int[intArrayLength];
                itemsAddedFromOther = new BitHelper(itemsAddedFromOtherPtr, intArrayLength);
            }
            else
            {
                int[] itemsToRemoveArray = new int[intArrayLength];
                itemsToRemove = new BitHelper(itemsToRemoveArray, intArrayLength);

                int[] itemsAddedFromOtherArray = new int[intArrayLength];
                itemsAddedFromOther = new BitHelper(itemsAddedFromOtherArray, intArrayLength);
            }

            foreach (T item in other)
            {
                int location = 0;
                bool added = AddOrGetLocation(item, out location);
                if (added)
                {
                    itemsAddedFromOther.MarkBit(location);
                }
                else
                {
                    if (location < originalLastIndex && !itemsAddedFromOther.IsMarked(location))
                    {
                        itemsToRemove.MarkBit(location);
                    }
                }
            }

            for (int i = 0; i < originalLastIndex; i++)
            {
                if (itemsToRemove.IsMarked(i))
                {
                    Remove(m_slots[i].value);
                }
            }
        }

        private bool AddOrGetLocation(T value, out int location)
        {
            Debug.Assert(m_buckets != null, "m_buckets is null, callers should have checked");

            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % m_buckets.Length;
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next)
            {
                if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, value))
                {
                    location = i;
                    return false; //already present
                }
            }
            int index;
            if (m_freeList >= 0)
            {
                index = m_freeList;
                m_freeList = m_slots[index].next;
            }
            else
            {
                if (m_lastIndex == m_slots.Length)
                {
                    IncreaseCapacity();
                    bucket = hashCode % m_buckets.Length;
                }
                index = m_lastIndex;
                m_lastIndex++;
            }
            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
            m_count++;
            m_version++;
            location = index;
            return true;
        }

        
        [SecuritySafeCritical]
        private unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount result;

            if (m_count == 0)
            {
                int numElementsInOther = 0;
                foreach (T item in other)
                {
                    numElementsInOther++;
                    break;
                }
                result.uniqueCount = 0;
                result.unfoundCount = numElementsInOther;
                return result;
            }


            Debug.Assert((m_buckets != null) && (m_count > 0), "m_buckets was null but count greater than 0");

            int originalLastIndex = m_lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            BitHelper bitHelper;
            if (intArrayLength <= StackAllocThreshold)
            {
                int* bitArrayPtr = stackalloc int[intArrayLength];
                bitHelper = new BitHelper(bitArrayPtr, intArrayLength);
            }
            else
            {
                int[] bitArray = new int[intArrayLength];
                bitHelper = new BitHelper(bitArray, intArrayLength);
            }

            int unfoundCount = 0;
            int uniqueFoundCount = 0;

            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    if (!bitHelper.IsMarked(index))
                    {
                        bitHelper.MarkBit(index);
                        uniqueFoundCount++;
                    }
                }
                else
                {
                    unfoundCount++;
                    if (returnIfUnfound)
                    {
                        break;
                    }
                }
            }

            result.uniqueCount = uniqueFoundCount;
            result.unfoundCount = unfoundCount;
            return result;
        }

        internal T[] ToArray()
        {
            T[] newArray = new T[Count];
            CopyTo(newArray);
            return newArray;
        }

        internal static bool HashSetEquals(HashSetSafe<T> set1, HashSetSafe<T> set2, IEqualityComparer<T> comparer)
        {
            if (set1 == null)
            {
                return (set2 == null);
            }
            else if (set2 == null)
            {
                return false;
            }

            if (AreEqualityComparersEqual(set1, set2))
            {
                if (set1.Count != set2.Count)
                {
                    return false;
                }
                foreach (T item in set2)
                {
                    if (!set1.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (T set2Item in set2)
                {
                    bool found = false;
                    foreach (T set1Item in set1)
                    {
                        if (comparer.Equals(set2Item, set1Item))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static bool AreEqualityComparersEqual(HashSetSafe<T> set1, HashSetSafe<T> set2)
        {
            return set1.Comparer.Equals(set2.Comparer);
        }

        private int InternalGetHashCode(T item)
        {
            if (item == null)
            {
                return 0;
            }
            return m_comparer.GetHashCode(item) & Lower31BitMask;
        }

        #endregion

        internal struct ElementCount
        {
            internal int uniqueCount;
            internal int unfoundCount;
        }

        internal struct Slot
        {
            internal int hashCode;
            internal int next;
            internal T value;
        }

#if !SILVERLIGHT
        [Serializable()]
        [HostProtection(MayLeakOnAbort = true)]
#endif
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private HashSetSafe<T> set;
            private int index;
            private int version;
            private T current;

            internal Enumerator(HashSetSafe<T> set)
            {
                this.set = set;
                index = 0;
                version = set.m_version;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (index < set.m_lastIndex)
                {
                    if (set.m_slots[index].hashCode >= 0)
                    {
                        current = set.m_slots[index].value;
                        index++;
                        return true;
                    }
                    index++;
                }
                index = set.m_lastIndex + 1;
                current = default(T);
                return false;
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == set.m_lastIndex + 1)
                    {
                        throw new InvalidOperationException();
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = default(T);
            }
        }
    }
}