using System;
using System.Diagnostics;
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

        /// <summary>
        /// 单位尺寸小于等于1024字节
        /// </summary>
        private static IntPtr* CacheLookUpTableSmall;

        /// <summary>
        /// 单位尺寸大于1024字节
        /// </summary>
        private static IntPtr* CacheLookUpTableBig;
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
                IsInited = true;
            }
            
        }

        public static void* Alloc(nuint size)
        {
            Debug.Assert(IsInited);
            return null;
        }

        public static void Free(void* ptr)
        {
            Debug.Assert(IsInited);
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

            InitSmallSlabs();
            if (MaxAllocSize>1024)
            {
                InitBigSlabs();
            }
            
            void InitSmallSlabs()
            {
                int lastFibIndex = 0;

                for (int i = 8; i <= 1024 && i<= MaxAllocSize; i+=8)
                {
                    int index = i / 8;
                    if (fibList[lastFibIndex]<i)
                    {
                        lastFibIndex++;
                    }
                    CacheLookUpTableSmall[index] = cacheList[lastFibIndex];
                }
            }

            void InitBigSlabs()
            {
                int lastFibIndex = fibList.First(x => x >= 1024);

                for (int i = 1024; i <= 1024 && i<= MaxAllocSize; i+=8)
                {
                    int index = i / 8;
                    if (fibList[lastFibIndex]<i)
                    {
                        lastFibIndex++;
                    }
                    CacheLookUpTableSmall[index] = cacheList[lastFibIndex];
                }
            }
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
    }
}

