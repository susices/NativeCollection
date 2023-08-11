using System.Collections;

namespace NativeCollection;

public unsafe struct MultiMapPair<T,K> : IEquatable<MultiMapPair<T,K>>,IComparable<MultiMapPair<T,K>>,IDisposable where T: unmanaged,IEquatable<T>,IComparable<T> where K:unmanaged,IEquatable<K>
{
    public readonly T Key;

    public readonly Internal.List<K>* _value;

    public Internal.List<K> Value => *_value;

    private MultiMapPair(T key)
    {
        Key = key;
        _value = Internal.List<K>.Create();
    }

    public static MultiMapPair<T, K> Create(T key)
    {
        MultiMapPair<T, K> pair = new MultiMapPair<T, K>(key);
        return pair;
    }

    public bool Equals(MultiMapPair<T, K> other)
    {
        return Key.Equals(other.Key);
    }

    public int CompareTo(MultiMapPair<T, K> other)
    {
        return Key.CompareTo(other.Key);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public void Dispose()
    {
        Console.WriteLine("MultiMapPair Dispose");
        _value->Dispose();
    }
}