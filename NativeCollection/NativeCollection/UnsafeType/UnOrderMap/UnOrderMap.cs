using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NativeCollection.UnsafeType
{
    public unsafe struct UnOrderMap<T,K> :IEnumerable<MapPair<T, K>>, IDisposable where T : unmanaged, IEquatable<T>, IComparable<T> where K : unmanaged, IEquatable<K>
{
    private UnsafeType.HashSet<MapPair<T, K>>* _hashSet;

    public static UnOrderMap<T, K>* Create(int initCapacity = 0)
    {
        UnOrderMap<T, K>* unOrderMap =(UnOrderMap<T, K>*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<UnOrderMap<T, K>>());
        unOrderMap->_hashSet = UnsafeType.HashSet<MapPair<T, K>>.Create(initCapacity);
        return unOrderMap;
    }

    public K this[T key]
    {
        get
        {
            var keyPair = new MapPair<T, K>(key);
            bool contains = _hashSet->TryGetValue(keyPair, out var pair);
            if (contains)
            {
                return pair.Value;
            }
            return default;
        }

        set
        {
            var pair = new MapPair<T, K>(key,value);
            var pairPtr = _hashSet->GetValuePointer(pair);
            if (pairPtr!=null)
            {
                pairPtr->_value = value;
            }
            else
            {
                _hashSet->Add(pair);
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(in T key, K value)
    {
        var mapPair = new MapPair<T, K>(key,value);
        _hashSet->AddRef(mapPair);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(in T key)
    {
        return _hashSet->ContainsRef(new MapPair<T, K>(key));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(in T key)
    {
        return _hashSet->RemoveRef(new MapPair<T, K>(key));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(in T key, out K value)
    {
        bool contains =  _hashSet->TryGetValue(new MapPair<T, K>(key), out var actualValue);
        if (contains)
        {
            value = actualValue.Value;
            return true;
        }
        value = default;
        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _hashSet->Clear();
    }
    
    public int Count => _hashSet->Count;
    
    IEnumerator<MapPair<T, K>> IEnumerable<MapPair<T, K>>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnsafeType.HashSet<MapPair<T, K>>.Enumerator GetEnumerator()
    {
        return new UnsafeType.HashSet<MapPair<T, K>>.Enumerator(_hashSet);
    }

    public void Dispose()
    {
        if (_hashSet!=null)
        {
            _hashSet->Dispose();
            NativeMemoryHelper.Free(_hashSet);
            NativeMemoryHelper.RemoveNativeMemoryByte(Unsafe.SizeOf<UnsafeType.HashSet<MapPair<T, K>>>());
        }
    }
}
}

