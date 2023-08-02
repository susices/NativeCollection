using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection;

public unsafe class Stack<T> : IDisposable where T : unmanaged, IEquatable<T>
{
    private const int _defaultCapacity = 10;
    private Internal.Stack<T>* _stack;
    public  Stack(int initialCapacity = _defaultCapacity)
    {
        _stack = Internal.Stack<T>.Create(initialCapacity);
    }

    public int Count => _stack->Count;

    public void Clear()
    {
        _stack->Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in T obj)
    {
        return _stack->Contains(obj);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peak()
    {
        return _stack->Peak();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        return _stack->Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPop(out T result)
    {
        bool returnValue = _stack->TryPop(out result);
        return returnValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(in T obj)
    {
        _stack->Push(obj);
    }

    public void Dispose()
    {
        if (_stack != null)
        {
            _stack->Dispose();
            NativeMemory.Free(_stack);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<Internal.Stack<T>>());
        }
    }

    ~Stack()
    {
        Dispose();
    }
}