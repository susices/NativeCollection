using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection.Internal;

internal unsafe struct Stack<T> : IDisposable where T : unmanaged, IEquatable<T>
{
    private T* _array;
    private int _version;
    private const int _defaultCapacity = 10;
    internal int ArrayLength { get; private set; }

    public static Stack<T>* Create(int initialCapacity = _defaultCapacity)
    {
        if (initialCapacity < 0) ThrowHelper.StackInitialCapacityException();

        var stack = (Stack<T>*)NativeMemory.Alloc((uint)Unsafe.SizeOf<Stack<T>>());
        GC.AddMemoryPressure(Unsafe.SizeOf<Stack<T>>());

        if (initialCapacity < _defaultCapacity)
            initialCapacity = _defaultCapacity; // Simplify doubling logic in Push.

        stack->_array = (T*)NativeMemory.Alloc((uint)initialCapacity, (uint)Unsafe.SizeOf<T>());
        GC.AddMemoryPressure(initialCapacity * Unsafe.SizeOf<T>());
        stack->ArrayLength = initialCapacity;
        stack->Count = 0;
        stack->_version = 0;
        return stack;
    }

    public int Count { get; private set; }

    
    public void Clear()
    {
        ArrayLength = 0;
        Count = 0;
        _version++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in T obj)
    {
        var count = Count;
        while (count-- > 0)
            if (obj.Equals(_array[count]))
                return true;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peak()
    {
        if (Count == 0) ThrowHelper.StackEmptyException();
        return _array[Count - 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        if (Count == 0)
            ThrowHelper.StackEmptyException();

        _version++;
        var obj = _array[--Count];
        _array[Count] = default;
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPop(out T result)
    {
        var index = Count - 1;
        var array = _array;
        if ((uint)index >= (uint)ArrayLength)
        {
            result = default;
            return false;
        }

        ++_version;
        Count = index;
        result = array[index];
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(in T obj)
    {
        if (Count == ArrayLength)
        {
            var newArray = (T*)NativeMemory.Alloc((uint)ArrayLength * 2, (uint)Unsafe.SizeOf<T>());
            GC.AddMemoryPressure(ArrayLength * 2 * Unsafe.SizeOf<T>());
            Unsafe.CopyBlockUnaligned(newArray, _array, (uint)(Count * Unsafe.SizeOf<T>()));
            NativeMemory.Free(_array);
            GC.RemoveMemoryPressure(ArrayLength * Unsafe.SizeOf<T>());
            _array = newArray;
            ArrayLength = ArrayLength * 2;
        }

        _array[Count++] = obj;
        _version++;
    }

    public void Dispose()
    {
        NativeMemory.Free(_array);
        GC.RemoveMemoryPressure(ArrayLength * Unsafe.SizeOf<T>());
    }
}