using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NativeCollection
{
    public static unsafe class MemoryAllocator
    {
        public static uint MinAllocSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            private set;
        }

        public static uint MaxAllocSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
            private set;
        }

        public static bool UsedInMultiThread
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; private set;
        }
        
        private static bool IsInited;

        private static object LockObjInit = new object();

        private static System.Collections.Generic.List<object> CacheLockObjs;

        private static uint CachedSlabCount;

        /// <summary>
        /// 单位尺寸小于等于1024字节的Cache
        /// </summary>
        private static MemoryCache** CacheTableSmall;
        private static uint CacheTableSmallLength;

        /// <summary>
        /// 单位尺寸大于1024字节的Cache
        /// </summary>
        private static MemoryCache** CacheTableBig;
        private static uint CacheTableBigLength;

        /// <summary>
        /// 内存分配器中 所有已分配的内存大小
        /// </summary>
        private static long UsedMemorySize;

        /// <summary>
        /// 内存分配器中 未分配的内存大小
        /// </summary>
        private static long FreeMemorySize;
        
        // private static int[] Z = { 24, 40, 48, 56, 80, 96, 112, 160, 192, 224, 330, -1 };
        
        /// <summary>
        /// 内存分配器初始化
        /// </summary>
        /// <param name="minSize">申请内存的最小长度 小于这个值时会申请该值大小的内存</param>
        /// <param name="maxSize">申请内存的最大长度 超过时会调用malloc申请</param>
        /// <param name="cachedSlabCount">最大可缓存的空闲Slab数</param>
        public static void Init(uint minSize, uint maxSize, uint cachedSlabCount = 3 , bool usedInMultiThread = true)
        {
            if (IsInited)
            {
                return;
            }

            lock (LockObjInit)
            {
                MinAllocSize = Math.Max(8,MemoryAllocatorHelper.RoundTo(minSize,8)) ;
                MaxAllocSize = MemoryAllocatorHelper.RoundTo(maxSize,256);
                CachedSlabCount = cachedSlabCount;
                UsedInMultiThread = usedInMultiThread;
                CacheLockObjs = new System.Collections.Generic.List<object>();
                UsedMemorySize = 0;
                FreeMemorySize = 0;
                InitCaches();
                IsInited = true;
            }
        }

        /// <summary>
        /// 申请指定长度的内存
        /// </summary>
        public static void* Alloc(uint size)
        {
            TryDefaultInit();
            return InternalAlloc(size, false);
        }

        /// <summary>
        /// 申请指定长度的内存 并初始化字节为0
        /// </summary>
        public static void* AllocZeroed(uint size)
        {
            return InternalAlloc(size, true);
        }
        
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void Free(void* freePrt)
        {
            TryDefaultInit();
            var listNodeSize = Unsafe.SizeOf<MemoryCache.ListNode>();
            var listNode = *(MemoryCache.ListNode*)((byte*)freePrt - listNodeSize);
            if (listNode.ParentSlab==null)
            {
                byte* realPtr = (byte*)freePrt - listNodeSize - Unsafe.SizeOf<nuint>();
                nuint size = *(nuint*)realPtr;
                NativeMemoryHelper.Free(realPtr);
                NativeMemoryHelper.RemoveNativeMemoryByte((long)size);
                Interlocked.Add(ref UsedMemorySize, -(long)size);
                return;
            }
            
            var cache = GetCacheByItemSize(listNode.ParentSlab->ItemSize);
            Debug.Assert(cache->ItemSize == listNode.ParentSlab->ItemSize);
            Debug.Assert(cache!=null);
            cache->Free(freePrt);
            Interlocked.Add(ref UsedMemorySize, -cache->ItemSize);
        }

        /// <summary>
        /// 查询指针对应的内存长度
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static long SizeOf(void* ptr)
        {
            TryDefaultInit();
            var listNodeSize = Unsafe.SizeOf<MemoryCache.ListNode>();
            var listNode = *(MemoryCache.ListNode*)((byte*)ptr - listNodeSize);
            if (listNode.ParentSlab==null)
            {
                byte* realPtr = (byte*)ptr - listNodeSize - Unsafe.SizeOf<nuint>();
                nuint size = *(nuint*)realPtr;
                return (long)size - listNodeSize - Unsafe.SizeOf<nuint>();
            }
            return listNode.ParentSlab->ItemSize;
        }

        /// <summary>
        /// 释放所有空闲内存
        /// </summary>
        public static void ReleaseAllUnusedMemory()
        {
            TryDefaultInit();
            ReleaseCaches(CacheTableSmall, CacheTableSmallLength);
            ReleaseCaches(CacheTableBig, CacheTableBigLength);
            
            void ReleaseCaches(MemoryCache** caches, uint cachesCount)
            {
                if (caches==null || cachesCount==0)
                {
                    return;
                }
                MemoryCache* cache = null;
                for (int i = 0; i < cachesCount; i++)
                {
                    var currentCache = caches[i];
                    if (currentCache==null || currentCache == cache)
                    {
                        continue;
                    }
                    cache = currentCache;
                    currentCache->ReleaseUnUsedSlabs();
                }
            }
        }

        public static long GetUsedMemorySize()
        {
            return UsedMemorySize;
        }
        
        /// <summary>
        /// 大尺寸对象内存申请
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* AllocBigObj(nuint size,bool zeroed)
        {
            var listNodeSize = Unsafe.SizeOf<MemoryCache.ListNode>();
            var realSize = size + (nuint)(listNodeSize + Unsafe.SizeOf<nuint>());
            byte* ptr;
            if (zeroed)
            {
                ptr =  (byte*)NativeMemoryHelper.AllocZeroed(realSize);
            }
            else
            {
                ptr =  (byte*)NativeMemoryHelper.Alloc(realSize);
            }
            
            *(nuint*)ptr = realSize;
            ptr += Unsafe.SizeOf<nuint>();
            ((MemoryCache.ListNode*)ptr)->ParentSlab = null;
            ptr += listNodeSize;
            return ptr;
        }

        /// <summary>
        /// 初始化Caches
        /// cache长度根据斐波那契数列增长
        /// </summary>
        private static void InitCaches()
        {
            uint fib1 = 8, fib2 = 16, f = 0;
            System.Collections.Generic.List<uint> fibList = new System.Collections.Generic.List<uint>();
            System.Collections.Generic.List<IntPtr> cacheList = new System.Collections.Generic.List<IntPtr>();
            
            fibList.Add(fib1);
            cacheList.Add((IntPtr)MemoryCache.CreateInternal(CalDefaultBlockSize(fib1),fib1,CachedSlabCount));
            fibList.Add(fib2);
            cacheList.Add((IntPtr)MemoryCache.CreateInternal(CalDefaultBlockSize(fib2),fib2,CachedSlabCount));
            do
            {
                f = fib1 + fib2;
                fib1 = fib2;
                fib2 = f;
                fibList.Add(f);  
                cacheList.Add((IntPtr)MemoryCache.CreateInternal(CalDefaultBlockSize(f),f,CachedSlabCount));
            } while (f < MaxAllocSize);
            
            InitCacheTable(ref CacheTableSmall,ref CacheTableSmallLength,Math.Min(1024, MaxAllocSize),8);
            if(MaxAllocSize > 1024 ) InitCacheTable(ref CacheTableBig,ref CacheTableBigLength, Math.Max(MaxAllocSize,fibList.Last()), 256);

            void InitCacheTable(ref MemoryCache** cacheTable,ref uint cacheTableLength, uint endSize, uint timesNum)
            {
                int lastFibIndex = 0;
                cacheTableLength = (uint)Unsafe.SizeOf<IntPtr>() * (endSize / timesNum + 1);
                cacheTable = (MemoryCache**)NativeMemoryHelper.AllocZeroed((nuint)cacheTableLength);
                for (uint i = 0; i <=endSize; i+=timesNum) 
                {
                    uint index = i / timesNum;
                    while (fibList[lastFibIndex]<i)
                    {
                        lastFibIndex++;
                    }
                    cacheTable[index] = (MemoryCache*)cacheList[lastFibIndex];
                }
            }
        }

        /// <summary>
        /// 根据对象长度查询对应的Cache
        /// </summary>
        /// <param name="itemSize"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MemoryCache* GetCacheByItemSize(uint itemSize)
        {
            MemoryCache* memoryCache = null;
            if (itemSize<=1024)
            {
                var index = itemSize / 8;
                memoryCache = CacheTableSmall[index];
            }
            else
            {
                var index = itemSize / 256;
                memoryCache = CacheTableBig[index];
            }
            
            Debug.Assert(memoryCache!=null);
            return memoryCache;
        }
        
        /// <summary>
        /// 根据对象尺寸 计算Slab的Block
        /// </summary>
        /// <param name="itemSize"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint CalDefaultBlockSize(uint itemSize)
        {
            if (itemSize<=1024)
            {
                return 4096/itemSize;
            }
            // 大对象 控制slab尺寸 要分组 尺寸又不能太大
            return 4;
        }

        /// <summary>
        /// 获取Cache中lock对象的指针
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IntPtr GetCacheLockObjPtr()
        {
            object lockObj = new object();
            CacheLockObjs.Add(lockObj);
            return GCHandle.ToIntPtr(GCHandle.Alloc(lockObj, GCHandleType.Pinned));
        }

        /// <summary>
        /// 默认初始化方法
        /// 未手动初始化时执行
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void TryDefaultInit()
        {
            if (!IsInited)
            {
                Init(8,8*1024,3,true);
            }
        }

        private static void* InternalAlloc(uint size, bool zeroed)
        {
            if (size==0) size = 1;
            if (size>MaxAllocSize)
            {
                return AllocBigObj(size,zeroed);
            }
            var cache = GetCacheByItemSize(size);
            Debug.Assert(cache!=null);
            var ptr = cache->Alloc();
            if (zeroed)
            {
                Unsafe.InitBlockUnaligned(ptr,0,size);
            }
            Interlocked.Add(ref UsedMemorySize, size);
            return ptr;
        }
    }
}

