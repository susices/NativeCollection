using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkHashSet
{
    [Params(10000,100000,1000000)]
    public int Count { get; set; }
    private List<int> input;
    private NativeCollection.HashSet<int> nativesHashSet;
    private HashSet<int> managedHashSet;
    
    
    [GlobalSetup(Targets = new []{nameof(NativeAddRemove),nameof(ManagedAddRemove)})]
    public void InitAddRemove()
    {
        nativesHashSet = new NativeCollection.HashSet<int>(1000);
        managedHashSet = new HashSet<int>();
        input = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            input.Add(Random.Shared.Next(Count));
        }
        foreach (var value in input)
        {
            nativesHashSet.Add(value);
        }
        foreach (var value in input)
        {
            managedHashSet.Add(value);
        }
    }

    [BenchmarkCategory("AddRemove")]
    [Benchmark]
    public void NativeAddRemove()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = Count; j < Count+1000; j++)
            {
                nativesHashSet.Add(j);
            }
            
            for (int j = Count; j < Count+1000; j++)
            {
                nativesHashSet.Remove(j);
            }
        }
    }
    
    [BenchmarkCategory("AddRemove")]
    [Benchmark(Baseline = true)]
    public void ManagedAddRemove()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = Count; j < Count+1000; j++)
            {
                managedHashSet.Add(j);
            }
            
            for (int j = Count; j < Count+1000; j++)
            {
                managedHashSet.Remove(j);
            }
        }
    }
    
    [GlobalSetup(Targets = new []{nameof(NativeEnumerate),nameof(ManagedEnumerate)})]
    public void InitEnumerate()
    {
        nativesHashSet = new NativeCollection.HashSet<int>();
        managedHashSet = new HashSet<int>();
        input = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            input.Add(Random.Shared.Next());
        }
        foreach (var value in input)
        {
            nativesHashSet.Add(value);
        }
        foreach (var value in input)
        {
            managedHashSet.Add(value);
        }
    }
    
    [BenchmarkCategory("Enumerate")]
    [Benchmark]
    public void NativeEnumerate()
    {
        int result = 0;
        foreach (var value in nativesHashSet)
        {
            result += value;
        }
    }
    
    [BenchmarkCategory("Enumerate")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerate()
    {
        int result = 0;
        foreach (var value in managedHashSet)
        {
            result += value;
        }
    }
}