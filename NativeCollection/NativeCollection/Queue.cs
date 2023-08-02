using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection;

public unsafe class Queue<T> : IDisposable where T : unmanaged
{
    private Internal.Queue<T>* _queue;

    public Queue(int capacity = 10)
    {
        _queue = Internal.Queue<T>.Create(capacity);
    }

    public int Count => _queue->Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        _queue->Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(in T item)
    {
        _queue->Enqueue(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue()
    {
        return _queue->Dequeue();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T result)
    {
        var value = _queue->TryDequeue(out result);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peek()
    {
        return _queue->Peek();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeek(out T result)
    {
        var value = _queue->TryPeek(out result);
        return value;
    }
    
    public void Dispose()
    {
        if (_queue!=null)
        {
            _queue->Dispose();
            NativeMemory.Free(_queue);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<Internal.Queue<T>>());
        }
    }

    ~Queue()
    {
        Dispose();
    }
}