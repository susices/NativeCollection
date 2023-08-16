using System.Runtime.CompilerServices;

namespace NativeCollection.UnsafeType;

public unsafe struct MultiMapPair<T, K> : IEquatable<MultiMapPair<T, K>>, IComparable<MultiMapPair<T, K>>, IDisposable
    where T : unmanaged, IEquatable<T>, IComparable<T> where K : unmanaged, IEquatable<K>
{
    private UnsafeType.List<K>* _value;

    public T Key { get; private set; }

    public ref UnsafeType.List<K> Value => ref Unsafe.AsRef<UnsafeType.List<K>>(_value);

    public MultiMapPair(T key)
    {
        Key = key;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MultiMapPair<T, K> Create(T key)
    {
        var pair = new MultiMapPair<T, K>(key);
        pair._value = List<K>.Create();
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
    public void Dispose()
    {
        if (_value!=null)
        {
            _value->Dispose();
            NativeMemoryHelper.Free(_value);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<UnsafeType.List<K>>());
        }
    }
}