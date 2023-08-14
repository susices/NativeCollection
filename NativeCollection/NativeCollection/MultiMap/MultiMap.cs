using System.Collections;
using System.Runtime.CompilerServices;

namespace NativeCollection;

public unsafe class MultiMap<T, K> : IEnumerable<MultiMapPair<T, K>>, IDisposable
    where T : unmanaged, IEquatable<T>, IComparable<T> where K : unmanaged, IEquatable<K>
{
    private readonly SortedSet<MultiMapPair<T, K>>* _sortedSet;

    public MultiMap()
    {
        _sortedSet = SortedSet<MultiMapPair<T, K>>.Create();
    }

    public Span<K> this[T key] {
        get
        {
            var list = MultiMapPair<T, K>.Create(key);
            var node = _sortedSet->FindNode(list);
            if (node!=null)
            {
                return node->Item.Value.AsSpan();
            }
            return Span<K>.Empty;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T key, K value)
    {
        var list = MultiMapPair<T, K>.Create(key);
        var node = _sortedSet->FindNode(list);

        if (node != null)
            list = node->Item;
        else
            _sortedSet->Add(list);
        
        list.Value.Add(value);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T key, K value)
    {
        var list = MultiMapPair<T, K>.Create(key);
        var node = _sortedSet->FindNode(list);

        if (node == null) return false;
        list = node->Item;
        if (!list.Value.Remove(value)) return false;

        if (list.Value.Count == 0) Remove(key);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T key)
    {
        var list = MultiMapPair<T, K>.Create(key);
        var node = _sortedSet->FindNode(list);

        if (node == null) return false;
        list = node->Item;
        var sortedSetRemove = _sortedSet->Remove(list);
        list.Dispose();
        return sortedSetRemove;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<MultiMapPair<T, K>> IEnumerable<MultiMapPair<T, K>>.GetEnumerator()
    {
        return _sortedSet->GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _sortedSet->GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SortedSet<MultiMapPair<T, K>>.Enumerator GetEnumerator()
    {
        return new SortedSet<MultiMapPair<T, K>>.Enumerator(_sortedSet);
    }
    
    public void Dispose()
    {
        if (_sortedSet != null)
        {
            _sortedSet->Dispose();
        }
    }
    
    ~MultiMap()
    {
        Dispose();
        if (_sortedSet != null)
        {
            NativeMemoryHelper.Free(_sortedSet);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<SortedSet<MultiMapPair<T, K>>>());
        }
    }
}