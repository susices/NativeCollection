using System;
using System.Runtime.CompilerServices;

namespace NativeCollection.UnsafeType
{
    public interface IPool : IDisposable
    {
        public void OnReturnToPool();
        public void OnGetFromPool();
    }
    
    public unsafe struct NativeStackPool<T> : IDisposable where T: unmanaged,IPool
    {
        public int MaxSize { get; private set; }
        private Stack<IntPtr>* _stack;
        public static NativeStackPool<T>* Create(int maxPoolSize)
        {
            NativeStackPool<T>* pool = (NativeStackPool<T>*)MemoryAllocator.Alloc((uint)Unsafe.SizeOf<NativeStackPool<T>>());
            pool->_stack = Stack<IntPtr>.Create();
            pool->MaxSize = maxPoolSize;
            return pool;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* Alloc()
        {
            if (_stack->TryPop(out var itemPtr))
            {
                var item = (T*)itemPtr;
                item->OnGetFromPool();
                return item;
            }
            return null;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T* ptr)
        {
            if (_stack->Count>=MaxSize)
            {
                ptr->Dispose();
                MemoryAllocator.Free(ptr);
                return;
            }
            ptr->OnReturnToPool();
            _stack->Push(new IntPtr(ptr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPoolMax()
        {
            return _stack->Count >= MaxSize;
        }
    
        public void Clear()
        {
            while (_stack->TryPop(out var ptr))
            {
                T* item = (T*)ptr;
                item->Dispose();
                MemoryAllocator.Free(item);
            }
        }
    
        public void Dispose()
        {
            Clear();
            _stack->Dispose();
            MemoryAllocator.Free(_stack);
        }
    }
}

