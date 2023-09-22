using System;
using System.Runtime.CompilerServices;
using NativeCollection.UnsafeType;

namespace NativeCollection
{
    public unsafe class NativePool<T> : INativeCollectionClass where T: unmanaged,IEquatable<T>,IPool
    {
        private UnsafeType.NativeStackPool<T>* _nativePool;
        private const int _defaultPoolSize = 200;
        private int _poolSize;
        public NativePool(int maxPoolSize = _defaultPoolSize)
        {
            _poolSize = maxPoolSize;
            _nativePool = UnsafeType.NativeStackPool<T>.Create(_poolSize);
            IsDisposed = false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* Alloc()
        {
            return _nativePool->Alloc();
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T* ptr)
        {
            _nativePool->Return(ptr);
        }
        
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            if (_nativePool != null)
            {
                _nativePool->Dispose();
                MemoryAllocator.Free(_nativePool);
                _nativePool = null;
                IsDisposed = true;
            }
        }
        
        public void ReInit()
        {
            if (IsDisposed)
            {
                _nativePool = UnsafeType.NativeStackPool<T>.Create(_poolSize);
                IsDisposed = false;
            }
        }
    
        public bool IsDisposed { get; private set; }
    }
}

