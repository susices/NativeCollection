using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NativeCollection.UnsafeType;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkMemoryPool
{
    [BenchmarkCategory("Alloc")]
    [Benchmark]
    public void MemoryPoolAlloc()
    {
        unsafe
        {
            MemoryPool* memoryPool =MemoryPool.Create(32, 32);
            for (int i = 0; i < 10000; i++)
            {
                var ptr =  memoryPool->Alloc();
                int* value = (int*)ptr;
                *value = i;
            }

            memoryPool->Dispose();
        }
    }
    
    [BenchmarkCategory("Alloc")]
    [Benchmark(Baseline = true)]
    public void NativeMemoryAlloc()
    {
        unsafe
        {
            for (int i = 0; i < 10000; i++)
            {
                var ptr = NativeMemory.Alloc(32);
                int* value = (int*)ptr;
                *value = i;
            }
        }
    }
}