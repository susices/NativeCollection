using System;
using System.Runtime.CompilerServices;

namespace NativeCollection.UnsafeType
{
    public unsafe struct MultiMapPair<T, K> : IEquatable<MultiMapPair<T, K>>, IComparable<MultiMapPair<T, K>>
        where T : unmanaged, IEquatable<T>, IComparable<T> where K : unmanaged, IEquatable<K>
    {
        private UnsafeType.List<K>* _value;
        
        public T Key { get; private set; }
    
        public ref UnsafeType.List<K> Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ref Unsafe.AsRef<UnsafeType.List<K>>(_value); }
        }

        public MultiMapPair(T key)
        {
            Key = key;
            _value = null;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MultiMapPair<T, K> Create(in T key, MemoryPool* pool)
        {
            var pair = new MultiMapPair<T, K>(key);
            var list = List<K>.AllocFromMemoryPool(pool);
            pair._value = list;
            return pair;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MultiMapPair<T, K> other)
        {
            return Key.Equals(other.Key);
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(MultiMapPair<T, K> other)
        {
            return Key.CompareTo(other.Key);
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose(MemoryPool* pool)
        {
            if (_value!=null)
            {
                pool->Free((byte*)_value);
            }
        }
    }
}

