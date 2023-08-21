using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkUnOrderMap
{
    [Params(100,1000,10000,100000,1000000)]
    public int KeyCount { get; set; }
    
    private System.Collections.Generic.List<int> input;
    private NativeCollection.UnOrderMap<int, int> nativeMap;
    private Dictionary<int, int> managedMap;
    
    [GlobalSetup(Targets = new []{nameof(NativeAddAndRemove),nameof(ManagedAddAndRemove)})]
    public void InitAddAndRemove()  
    {
        nativeMap = new NativeCollection.UnOrderMap<int, int>(1000);
        managedMap = new Dictionary<int, int>();
        input = new System.Collections.Generic.List<int>();
        for (int i = 0; i < KeyCount; i++)
        {
            input.Add(i);
        }
        foreach (var key in input)
        {
            nativeMap.Add(key,key);
        }
        foreach (var key in input)
        {
            managedMap.Add(key,key);
        }
    }

    [BenchmarkCategory("AddAndRemove")]
    [Benchmark]
    public void NativeAddAndRemove()
    {
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            nativeMap.Add(m,1);
        }
        
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            nativeMap.Remove(m);
        }
    }

    [BenchmarkCategory("AddAndRemove")]
    [Benchmark(Baseline = true)]
    public void ManagedAddAndRemove()
    {
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            managedMap.Add(m,1);
        }
        
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            managedMap.Remove(m);
        }
    }
    
    [GlobalSetup(Targets = new []{nameof(NativeEnumerateAll),nameof(ManagedEnumerateAll),nameof(NativeEnumerateAndRemoveFirst10),nameof(ManagedEnumerateAndRemoveFirst10)})]
    public void InitEnumerate()
    {
        nativeMap = new NativeCollection.UnOrderMap<int, int>(1000);
        managedMap = new Dictionary<int, int>();
        input = new System.Collections.Generic.List<int>();
        for (int i = 0; i < KeyCount; i++)
        {
            input.Add(i);
        }
        foreach (var key in input)
        {
            nativeMap.Add(key,key);
        }
        foreach (var key in input)
        {
            managedMap.Add(key,key);
        }
    }
    
    [BenchmarkCategory("EnumerateAll")]
    [Benchmark]
    public void NativeEnumerateAll()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var pair in nativeMap)
            {
                var value = pair.Value;
            }
        }
    }
    
    
    [BenchmarkCategory("EnumerateAll")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerateAll()
    {

        for (int i = 0; i < 10; i++)
        {
            foreach (var pair in managedMap)
            {
                var value = pair.Value;
            }
        }
    }
    
    [BenchmarkCategory("EnumerateFirst10")]
    [Benchmark]
    public void NativeEnumerateAndRemoveFirst10()
    {
        using var enumerator = nativeMap.GetEnumerator();
        Span<int> list = stackalloc int[10];
        int index = 0;
        while (enumerator.MoveNext()&&index<10)
        {
            var pair = enumerator.Current;
            list[index] = pair.Key;
            index++;
        }

        foreach (var i in list)
        {
            nativeMap.Remove(i);
        }
    }

    [BenchmarkCategory("EnumerateFirst10")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerateAndRemoveFirst10()
    {
        using var enumerator = managedMap.GetEnumerator();
        Span<int> list = stackalloc int[10];
        int index = 0;
        while (enumerator.MoveNext()&&index<10)
        {
            var pair = enumerator.Current;
            list[index] = pair.Key;
            index++;
        }

        foreach (var i in list)
        {
            managedMap.Remove(i);
        }
    }
    
    [GlobalCleanup]
    public void Dispose()
    {
        nativeMap.Dispose();
    }
}