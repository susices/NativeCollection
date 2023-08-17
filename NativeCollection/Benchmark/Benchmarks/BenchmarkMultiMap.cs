using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NativeCollection.UnsafeType;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkMultiMap
{
    [Params(10000,100000,1000000)]
    public int KeyCount { get; set; }
    public int ValueCount { get; set; }
    private System.Collections.Generic.List<int> input;
    private NativeCollection.MultiMap<int, int> nativeMultiMap;
    private MultiMap<int, int> managedMultiMap;
    
    // [GlobalSetup(Targets = new []{nameof(NativeAddAllAndRemoveAll),nameof(ManagedAddAllAndRemoveAll)})]
    // public void InitAddAllAndRemoveAll()
    // {
    //     nativeMultiMap = new NativeCollection.MultiMap<int, int>(1000);
    //     managedMultiMap = new MultiMap<int, int>(1000);
    //     ValueCount = 5;
    //     input = new System.Collections.Generic.List<int>();
    //     for (int i = 0; i < KeyCount; i++)
    //     {
    //         input.Add(Random.Shared.Next());
    //     }
    // }
    //
    // [BenchmarkCategory("AddAllAndRemoveAll")]
    // [Benchmark]
    // public void NativeAddAllAndRemoveAll()
    // {
    //     for (int j = 0; j < 10; j++)
    //     {
    //         foreach (var key in input)
    //         {
    //             for (int i = 0; i < ValueCount; i++)
    //             {
    //                 nativeMultiMap.Add(key,i);
    //             }
    //         }
    //
    //         foreach (var key in input)
    //         {
    //             for (int i = 0; i < ValueCount; i++)
    //             {
    //                 nativeMultiMap.Remove(key,i);
    //             }
    //         }
    //         nativeMultiMap.Clear();
    //     }
    // }
    //
    // [BenchmarkCategory("AddAllAndRemoveAll")]
    // [Benchmark(Baseline = true)]
    // public void ManagedAddAllAndRemoveAll()
    // {
    //
    //     for (int j = 0; j < 10; j++)
    //     {
    //         foreach (var key in input)
    //         {
    //             for (int i = 0; i < ValueCount; i++)
    //             {
    //                 managedMultiMap.Add(key,i);
    //             }
    //         }
    //
    //         foreach (var key in input)
    //         {
    //             for (int i = 0; i < ValueCount; i++)
    //             {
    //                 managedMultiMap.Remove(key,i);
    //             }
    //         }
    //         managedMultiMap.Clear();
    //     }
    // }

    [GlobalSetup(Targets = new []{nameof(NativeAddAndRemove),nameof(ManagedAddAndRemove)})]
    public void InitAddAndRemove()
    {
        nativeMultiMap = new NativeCollection.MultiMap<int, int>(1000);
        managedMultiMap = new MultiMap<int, int>(1000);
        ValueCount = 5;
        input = new System.Collections.Generic.List<int>();
        for (int i = 0; i < KeyCount; i++)
        {
            input.Add(Random.Shared.Next());
        }
        foreach (var key in input)
        {
            for (int i = 0; i < ValueCount; i++)
            {
                nativeMultiMap.Add(key,i);
            }
        }
        foreach (var key in input)
        {
            for (int i = 0; i < ValueCount; i++)
            {
                managedMultiMap.Add(key,i);
            }
        }
    }

    [BenchmarkCategory("AddAndRemove")]
    [Benchmark]
    public void NativeAddAndRemove()
    {
        for (int m = 0; m < 100; m++)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    nativeMultiMap.Add(i,j);
                }
                for (int j = 0; j < 10; j++)
                {
                    nativeMultiMap.Remove(i,j);
                }
            }
        }
    }

    [BenchmarkCategory("AddAndRemove")]
    [Benchmark(Baseline = true)]
    public void ManagedAddAndRemove()
    {
        for (int m = 0; m < 100; m++)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    managedMultiMap.Add(i,j);
                }
                for (int j = 0; j < 10; j++)
                {
                    managedMultiMap.Remove(i,j);
                }
            }
        }
    }


    // [GlobalSetup(Targets = new []{nameof(NativeEnumerate),nameof(ManagedEnumerate)})]
    // public void InitEnumerate()
    // {
    //     nativeMultiMap = new NativeCollection.MultiMap<int, int>();
    //     managedMultiMap = new MultiMap<int, int>(1000);
    //     ValueCount = 5;
    //     input = new System.Collections.Generic.List<int>();
    //     for (int i = 0; i < KeyCount; i++)
    //     {
    //         input.Add(Random.Shared.Next());
    //     }
    //     foreach (var key in input)
    //     {
    //         for (int i = 0; i < ValueCount; i++)
    //         {
    //             nativeMultiMap.Add(key,i);
    //         }
    //     }
    //     foreach (var key in input)
    //     {
    //         for (int i = 0; i < ValueCount; i++)
    //         {
    //             managedMultiMap.Add(key,i);
    //         }
    //     }
    // }
    //
    // [BenchmarkCategory("Enumerate")]
    // [Benchmark]
    // public void NativeEnumerate()
    // {
    //     using var enumerator = nativeMultiMap.GetEnumerator();
    //     while (enumerator.MoveNext())
    //     {
    //         var pair = enumerator.Current;
    //         
    //         foreach (var listValue in pair.Value)
    //         {
    //             var value = listValue;
    //         }
    //     }
    // }
    //
    //
    // [BenchmarkCategory("Enumerate")]
    // [Benchmark(Baseline = true)]
    // public void ManagedEnumerate()
    // {
    //     using var enumerator = managedMultiMap.GetEnumerator();
    //     while (enumerator.MoveNext())
    //     {
    //         var pair = enumerator.Current;
    //         foreach (var listValue in pair.Value)
    //         {
    //             var value = listValue;
    //         }
    //     }
    // }
    
    [GlobalCleanup]
    public void Dispose()
    {
        nativeMultiMap.Dispose();
    }
}

public class SortedCollection<K, V>: SortedDictionary<K, V>
{
}

public class MultiMap<T, K>: SortedCollection<T, System.Collections.Generic.List<K>>
    {
        private readonly System.Collections.Generic.List<K> Empty = new();
        private readonly int maxPoolCount;
        private readonly System.Collections.Generic.Queue<System.Collections.Generic.List<K>> pool;

        public MultiMap(int maxPoolCount = 0)
        {
            this.maxPoolCount = maxPoolCount;
            this.pool = new System.Collections.Generic.Queue<System.Collections.Generic.List<K>>(maxPoolCount);
        }

        private System.Collections.Generic.List<K> FetchList()
        {
            if (this.pool.Count > 0)
            {
                return this.pool.Dequeue();
            }
            return new System.Collections.Generic.List<K>(10);
        }

        private void Recycle(System.Collections.Generic.List<K> list)
        {
            if (list == null)
            {
                return;
            }
            if (this.pool.Count == this.maxPoolCount)
            {
                return;
            }
            list.Clear();
            this.pool.Enqueue(list);
        }

        public void Add(T t, K k)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.Add(t, list);
            }
            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        public new bool Remove(T t)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            this.Recycle(list);
            return base.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return Array.Empty<K>();
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new System.Collections.Generic.List<K> this[T t]
        {
            get
            {
                this.TryGetValue(t, out System.Collections.Generic.List<K> list);
                return list ?? Empty;
            }
        }

        public K GetOne(T t)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        public bool Contains(T t, K k)
        {
            System.Collections.Generic.List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }
    }