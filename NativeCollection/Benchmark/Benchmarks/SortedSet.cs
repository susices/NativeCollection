using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class SortedSet
{
    [Params(1000,10000,100000)]
    public int Count { get; set; }
    private List<int> input;
    private NativeCollection.SortedSet<int> nativesSortedSet;
    private SortedSet<int> managedSortedSet;
    
    
    [GlobalSetup(Targets = new []{nameof(NativeAddRemove),nameof(ManagedAddRemove)})]
    public void InitAddRemove()
    {
        nativesSortedSet = new NativeCollection.SortedSet<int>();
        managedSortedSet = new SortedSet<int>();
        input = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            input.Add(Random.Shared.Next());
        }
    }

    [BenchmarkCategory("AddRemove")]
    [Benchmark]
    public void NativeAddRemove()
    {
        foreach (var value in input)
        {
            nativesSortedSet.Add(value);
        }
        foreach (var value in input)
        {
            nativesSortedSet.Remove(value);
        }
    }
    
    [BenchmarkCategory("AddRemove")]
    [Benchmark(Baseline = true)]
    public void ManagedAddRemove()
    {
        foreach (var value in input)
        {
            managedSortedSet.Add(value);
        }
        foreach (var value in input)
        {
            managedSortedSet.Remove(value);
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