using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkSortedSet
{
    [Params(10000,100000,1000000)]
    public int Count { get; set; }
    private List<int> input;
    private NativeCollection.SortedSet<int> nativesSortedSet;
    private SortedSet<int> managedSortedSet;
    
    
    [GlobalSetup(Targets = new []{nameof(NativeAddRemove),nameof(ManagedAddRemove)})]
    public void InitAddRemove()
    {
        nativesSortedSet = new NativeCollection.SortedSet<int>(1000);
        managedSortedSet = new SortedSet<int>();
        input = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            input.Add(Random.Shared.Next(Count));
        }
        foreach (var value in input)
        {
            nativesSortedSet.Add(value);
        }
        foreach (var value in input)
        {
            managedSortedSet.Add(value);
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
                nativesSortedSet.Add(j);
            }
            
            for (int j = Count; j < Count+1000; j++)
            {
                nativesSortedSet.Remove(j);
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
                managedSortedSet.Add(j);
            }
            
            for (int j = Count; j < Count+1000; j++)
            {
                managedSortedSet.Remove(j);
            }
        }
    }
    
    [GlobalSetup(Targets = new []{nameof(NativeEnumerate),nameof(ManagedEnumerate)})]
    public void InitEnumerate()
    {
        nativesSortedSet = new NativeCollection.SortedSet<int>();
        managedSortedSet = new SortedSet<int>();
        input = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            input.Add(Random.Shared.Next());
        }
        foreach (var value in input)
        {
            nativesSortedSet.Add(value);
        }
        foreach (var value in input)
        {
            managedSortedSet.Add(value);
        }
    }
    
    [BenchmarkCategory("Enumerate")]
    [Benchmark]
    public void NativeEnumerate()
    {
        int result = 0;
        foreach (var value in nativesSortedSet)
        {
            result += value;
        }
    }
    
    [BenchmarkCategory("Enumerate")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerate()
    {
        int result = 0;
        foreach (var value in managedSortedSet)
        {
            result += value;
        }
    }
}