using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NativeCollection
{
    public static unsafe class MemoryAllocator
    {
        public static int  MinAllocSize { get; private set; }
        public static int MaxAllocSize { get; private set; }
        
        private static bool IsInited;

        private static object LockObjInit;

        private static int CachedSlabCount;

        private static UnOrderMap<IntPtr, nuint> BigObjSizeMap ;

        /// <summary>
        /// 单位尺寸小于等于1024字节的Cache
        /// </summary>
        private static MemoryCache** CacheTableSmall;
        private static int CacheTableSmallLength;

        /// <summary>
        /// 单位尺寸大于1024字节的Cache
        /// </summary>
        private static MemoryCache** CacheTableBig;
        private static int CacheTableBigLength;
        
        private static int[] Z = { 24, 40, 48, 56, 80, 96, 112, 160, 192, 224, 330, -1 };
        
        public static void Init(int minSize, int maxSize, int cachedSlabCount = 3)
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
                InitCaches();
                BigObjSizeMap = new UnOrderMap<IntPtr, nuint>();
                IsInited = true;
            }
        }

        public static void* Alloc(nuint size)
        {
            Debug.Assert(IsInited);
            Debug.Assert(size>0);
            if (size>(nuint)MaxAllocSize)
            {
                return AllocBigObj(size);
            }

            var cache = GetCacheByItemSize(size);
            Debug.Assert(cache!=null);
            return cache->Alloc();
        }

        

        public static void Free(void* freePrt)
        {
            Debug.Assert(IsInited);
            Debug.Assert(freePrt!=null);
            Dictionary<int, int> dic = new Dictionary<int, int>();
            
            if (BigObjSizeMap.TryGetValue((IntPtr)freePrt, out var size))
            {
                BigObjSizeMap.Remove((IntPtr)freePrt);
                FreeBigObj((byte*)freePrt - Unsafe.SizeOf<MemoryCache.ListNode>(), size);
                return;
            }
            
            var listNodeSize = (nuint)Unsafe.SizeOf<MemoryCache.ListNode>();
            var listNode = (MemoryCache.ListNode*)((byte*)freePrt - listNodeSize);
            
            var cache = GetCacheByItemSize(size);
            Debug.Assert(cache!=null);
            cache->Free(freePrt);
        }
        
        /// <summary>
        /// 大尺寸对象内存申请
        /// </summary>
        private static void* AllocBigObj(nuint size)
        {
            var listNodeSize = (nuint)Unsafe.SizeOf<MemoryCache.ListNode>();
            var realSize = size + listNodeSize;
            byte* ptr =  (byte*)NativeMemoryHelper.Alloc(realSize);
            IntPtr intPtr = (IntPtr)ptr;
            intPtr = IntPtr.Zero;
            BigObjSizeMap.Add((IntPtr)ptr,realSize);
            return ptr+listNodeSize;
        }

        private static void FreeBigObj(void* freePtr, nuint size)
        {
            
            NativeMemoryHelper.Free(freePtr);
            NativeMemoryHelper.RemoveNativeMemoryByte((long)size);
        }

        /// <summary>
        /// 初始化Caches
        /// </summary>
        private static void InitCaches()
        {
            
            int fib1 = 8, fib2 = 16, f = 0;
            System.Collections.Generic.List<int> fibList = new System.Collections.Generic.List<int>();
            System.Collections.Generic.List<IntPtr> cacheList = new System.Collections.Generic.List<IntPtr>();
            
            fibList.Add(fib1);
            fibList.Add(fib2);
            f = fib1 + fib2;
            while (f <= MaxAllocSize)
            {
                fibList.Add(f);  
                cacheList.Add((IntPtr)MemoryCache.CreateInternal(CalDefaultBlockSize(f),f,CachedSlabCount));
                f = fib1 + fib2;
                fib1 = fib2;
                fib2 = f;
            }
            
            InitCacheTable(ref CacheTableSmall,ref CacheTableSmallLength,Math.Min(1024, MaxAllocSize),8);
            if(MaxAllocSize > 1024 ) InitCacheTable(ref CacheTableBig,ref CacheTableBigLength, MaxAllocSize, 256);

            void InitCacheTable(ref MemoryCache** cacheTable,ref int cacheTableLength, int endSize, int timesNum)
            {
                int lastFibIndex = 0;
                cacheTableLength = Unsafe.SizeOf<IntPtr>() * (endSize / timesNum + 1);
                cacheTable = (MemoryCache**)NativeMemoryHelper.AllocZeroed((nuint)cacheTableLength);
                for (int i = 0; i <=endSize; i+=timesNum) 
                {
                    int index = i / timesNum;
                    while (fibList[lastFibIndex]<i)
                    {
                        lastFibIndex++;
                    }
                    cacheTable[index] = (MemoryCache*)cacheList[lastFibIndex];
                }
            }
        }

        private static MemoryCache* GetCacheByItemSize(nuint itemSize)
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
                memoryCache = CacheTableSmall[index];
            }
            
            Debug.Assert(memoryCache!=null);
            return memoryCache;
        }

        

        /// <summary>
        /// 根据对象尺寸 计算Slab的Block
        /// </summary>
        /// <param name="itemSize"></param>
        /// <returns></returns>
        private static int CalDefaultBlockSize(int itemSize)
        {
            if (itemSize<=1024)
            {
                return 4096/itemSize;
            }
            
            // 大对象 控制slab尺寸 要分组 尺寸又不能太大
            return 4;
        }

        [DoesNotReturn]
        private static void NotFoundPtrSizeException()
        {
            throw new InvalidOperationException("NotFoundPtrSize");
        }
    }
}

