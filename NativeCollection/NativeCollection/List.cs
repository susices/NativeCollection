using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeCollection;

public unsafe class List<T>: ICollection<T>, INativeCollectionClass where T:unmanaged, IEquatable<T>
{
    private UnsafeType.List<T>* _list;
    private const int _defaultCapacity = 10;
    public List(int capacity = _defaultCapacity)
    {
        _list = UnsafeType.List<T>.Create(capacity);
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
    
    public UnsafeType.List<T>.Enumerator GetEnumerator()
    {
        return new UnsafeType.List<T>.Enumerator(_list);
    }

    public void Add(T item)
    {
        _list->Add(item);
    }

    public void Clear()
    {
        _list->Clear();
    }

    public bool Contains(T item)
    {
        return _list->Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list->CopyTo(array,arrayIndex);
    }

    public bool Remove(T item)
    {
        return _list->Remove(item);
    }

    public int Capacity
    {
        get => _list->Capacity;
        set => _list->Capacity = value;
    }
    public int IndexOf(in T item)
    {
        return _list->IndexOf(item);
    }
    
    public void RemoveAt(int index)
    {
        _list->RemoveAt(index);
    }

    public int Count => _list->Count;
    public bool IsReadOnly => false;
    
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        if (_list!=null)
        {
            _list->Dispose();
            NativeMemoryHelper.Free(_list);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<UnsafeType.List<T>>());
            IsDisposed = true;
        }
    }

    public void ReInit()
    {
        if (IsDisposed)
        {
            _list = UnsafeType.List<T>.Create(_defaultCapacity);
            IsDisposed = false;
        }
    }

    public bool IsDisposed { get; private set; }

    ~List()
    {
        Dispose();
    }
}