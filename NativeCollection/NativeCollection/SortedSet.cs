using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeCollection;

public unsafe class SortedSet<T> : ICollection<T>, INativeCollectionClass where T : unmanaged, IEquatable<T>,IComparable<T>
{
    private UnsafeType.SortedSet<T>* _sortedSet;
    public SortedSet()
    {
        _sortedSet = UnsafeType.SortedSet<T>.Create();
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
    
    public UnsafeType.SortedSet<T>.Enumerator GetEnumerator()
    {
        return new UnsafeType.SortedSet<T>.Enumerator(_sortedSet);
    }

    void ICollection<T>.Add(T item)
    {
        
    }

    public bool Add(T item)
    {
        return _sortedSet->Add(item);
    }

    public void Clear()
    {
        _sortedSet->Clear();
    }

    public bool Contains(T item)
    {
        return _sortedSet->Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _sortedSet->CopyTo(array,arrayIndex);
    }

    public bool Remove(T item)
    {
        return _sortedSet->Remove(item);
    }
    
    public T? Min => _sortedSet->Min;
    
    public T? Max => _sortedSet->Max;

    public int Count => _sortedSet->Count;
    public bool IsReadOnly => false;
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        if (_sortedSet!=null)
        {
            _sortedSet->Dispose();
            NativeMemoryHelper.Free(_sortedSet);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<UnsafeType.SortedSet<T>>());
            IsDisposed = true;
        }
    }

    public void ReInit()
    {
        if (IsDisposed)
        {
            _sortedSet = UnsafeType.SortedSet<T>.Create();
            IsDisposed = false;
        }
    }

    public bool IsDisposed { get; private set; }
}