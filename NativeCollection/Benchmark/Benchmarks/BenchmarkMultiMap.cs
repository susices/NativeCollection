using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NativeCollection.UnsafeType;

namespace Benchmark.Benchmarks;
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BenchmarkMultiMap
{
    [Params(100000)]
    public int KeyCount { get; set; }
    public int ValueCount { get; set; }
    private System.Collections.Generic.List<int> input;
    private NativeCollection.MultiMap<int, int> nativeMultiMap;
    private MultiMap<int, int> managedMultiMap;

    [GlobalSetup(Targets = new []{nameof(NativeAddAndRemove),nameof(ManagedAddAndRemove)})]
    public void InitAddAndRemove()
    {
        nativeMultiMap = new NativeCollection.MultiMap<int, int>(1000);
        managedMultiMap = new MultiMap<int, int>(1000);
        ValueCount = 5;
        input = new System.Collections.Generic.List<int>();
        for (int i = 0; i < KeyCount; i++)
        {
            input.Add(i);
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
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            nativeMultiMap.Add(m,1);
        }
        
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            nativeMultiMap.Remove(m,1);
        }
    }

    [BenchmarkCategory("AddAndRemove")]
    [Benchmark(Baseline = true)]
    public void ManagedAddAndRemove()
    {
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            managedMultiMap.Add(m,1);
        }
        
        for (int m = KeyCount; m <KeyCount+ 1000; m++)
        {
            managedMultiMap.Remove(m,1);
        }
    }


    [GlobalSetup(Targets = new []{nameof(NativeEnumerateAll),nameof(ManagedEnumerateAll),nameof(NativeEnumerateAndRemoveFirst10),nameof(ManagedEnumerateAndRemoveFirst10)})]
    public void InitEnumerate()
    {
        nativeMultiMap = new NativeCollection.MultiMap<int, int>(1000);
        managedMultiMap = new MultiMap<int, int>(1000);
        ValueCount = 5;
        input = new System.Collections.Generic.List<int>();
        for (int i = 0; i < KeyCount; i++)
        {
            input.Add(i);
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
    
    [BenchmarkCategory("EnumerateAll")]
    [Benchmark]
    public void NativeEnumerateAll()
    {
        using var enumerator = nativeMultiMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var pair = enumerator.Current;
            
            foreach (var listValue in pair.Value)
            {
                var value = listValue;
            }
        }
    }
    
    
    [BenchmarkCategory("EnumerateAll")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerateAll()
    {
        using var enumerator = managedMultiMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var pair = enumerator.Current;
            foreach (var listValue in pair.Value)
            {
                var value = listValue;
            }
        }
    }


    [BenchmarkCategory("EnumerateFirst10")]
    [Benchmark]
    public void NativeEnumerateAndRemoveFirst10()
    {
        using var enumerator = nativeMultiMap.GetEnumerator();
        Span<int> list = stackalloc int[10];
        int index = 0;
        while (enumerator.MoveNext()&&index<10)
        {
            var pair = enumerator.Current;
            list[index] = pair.Key;
            index++;
            foreach (var listValue in pair.Value)
            {
                var value = listValue;
            }
        }

        foreach (var i in list)
        {
            nativeMultiMap.Remove(i);
        }
    }

    [BenchmarkCategory("EnumerateFirst10")]
    [Benchmark(Baseline = true)]
    public void ManagedEnumerateAndRemoveFirst10()
    {
        using var enumerator = managedMultiMap.GetEnumerator();
        Span<int> list = stackalloc int[10];
        int index = 0;
        while (enumerator.MoveNext()&&index<10)
        {
            var pair = enumerator.Current;
            list[index] = pair.Key;
            index++;
            foreach (var listValue in pair.Value)
            {
                var value = listValue;
            }
        }

        foreach (var i in list)
        {
            managedMultiMap.Remove(i);
        }
    }
    
    
    
    
    
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