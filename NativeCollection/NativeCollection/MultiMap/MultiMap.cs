using System.Collections;

namespace NativeCollection;

public unsafe class MultiMap<T,K> : IEnumerable<MultiMapPair<T, K>>, IEnumerable where T: unmanaged,IEquatable<T>,IComparable<T> where K:unmanaged,IEquatable<K>
{
    private SortedSet<MultiMapPair<T, K>> _sortedSet;
    private Dictionary<T, MultiMapPair<T, K>> _multiMapPairDic;
    
    public MultiMap()
    {
        _sortedSet = new SortedSet<MultiMapPair<T, K>>();
        _multiMapPairDic = new Dictionary<T, MultiMapPair<T, K>>();
    }

    public void Add(T key, K value)
    {
        
        if (!_multiMapPairDic.TryGetValue(key, out MultiMapPair<T, K> list))
        {
            list = MultiMapPair<T, K>.Create(key);
            _multiMapPairDic.Add(key,list);
            _sortedSet.Add(list);
        }
        list._value->Add(value);
        
        Console.WriteLine($"multiMap Add key:{key} value:{value} list:{*list._value}");
    }

    public bool Remove(T key, K value)
    {
        if (!_multiMapPairDic.TryGetValue(key, out MultiMapPair<T, K> list))
        {
            return false;
        }

        if (!list.Value.Remove(value))
        {
            return false;
        }

        if (list.Value.Count==0)
        {
            Remove(key);
        }

        return true;
    }

    public bool Remove(T key)
    {
        if (!_multiMapPairDic.TryGetValue(key, out MultiMapPair<T, K> list))
        {
            return false;
        }
        
        bool dicRemove = _multiMapPairDic.Remove(key);
        bool sortedSetRemove =  _sortedSet.Remove(list);
        list.Dispose();
        return dicRemove && sortedSetRemove;
    }

    public Span<K> this[T key]
    {
        get
        {
            if (_multiMapPairDic.TryGetValue(key, out var list))
            {
                return list.Value.AsSpan();
            }
            return Span<K>.Empty;
        }
    }

    IEnumerator<MultiMapPair<T, K>> IEnumerable<MultiMapPair<T, K>>.GetEnumerator()
    {
        return _sortedSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _sortedSet.GetEnumerator();
    }
    
    public SortedSet<MultiMapPair<T, K>>.Enumerator GetEnumerator()
    {
        Console.WriteLine("GetEnumerator");
        return new SortedSet<MultiMapPair<T, K>>.Enumerator(_sortedSet);
    }
    
}