using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeCollection;

public unsafe class HashSet<T>: ICollection<T>, INativeCollectionClass where T : unmanaged, IEquatable<T>
{

    private UnsafeType.HashSet<T>* _hashSet;
    private const int _defaultCapacity = 10;
    public HashSet(int capacity = _defaultCapacity)
    {
        _hashSet = UnsafeType.HashSet<T>.Create(capacity);
        IsDisposed = false;
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public UnsafeType.HashSet<T>.Enumerator GetEnumerator() => new UnsafeType.HashSet<T>.Enumerator(_hashSet);

    public void Add(T item)
    {
        _hashSet->Add(item);
    }

    public void Clear()
    {
        _hashSet->Clear();
    }

    public bool Contains(T item)
    {
        return _hashSet->Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _hashSet->CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _hashSet->Remove(item);
    }

    public int Count => _hashSet->Count;
    public bool IsReadOnly => false;
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        if (_hashSet!=null)
        {
            _hashSet->Dispose();
            NativeMemoryHelper.Free(_hashSet);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<UnsafeType.HashSet<T>>());
            IsDisposed = true;
        }
    }

    public void ReInit()
    {
        if (IsDisposed)
        {
            _hashSet = UnsafeType.HashSet<T>.Create(_defaultCapacity);
            IsDisposed = false;
        }
    }

    public bool IsDisposed { get; private set; }

    ~HashSet()
    {
        Dispose();
    }
}