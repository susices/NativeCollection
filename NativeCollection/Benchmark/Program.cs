// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using NativeCollection;
using NativeCollection.UnsafeType;

public static class Program
{
    public static void Main()
    {
        // TestStack();
        //TestQueue();
        //TestSortedSet();
        //TestSortedSetAddRemove();
        //TestList();
        //TestMultiMap();
        //TestHashSet();

        var summary = BenchmarkRunner.Run<SortedSet>();
    }
    
    

    public static void TestStack()
    {
        var input = new System.Collections.Generic.List<int>(10000000);
        for (var i = 0; i < 10000000; i++) input.Add(Random.Shared.Next());

        var nativeStack = new NativeCollection.Stack<int>();
        var managedStack = new System.Collections.Generic.Stack<int>();

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) nativeStack.Push(value);

            while (nativeStack.Count != 0) nativeStack.Pop();
            stopwatch.Stop();
            Console.WriteLine($"native stack : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) managedStack.Push(value);

            while (managedStack.Count != 0) managedStack.Pop();
            stopwatch.Stop();
            Console.WriteLine($"managed stack : {stopwatch.ElapsedMilliseconds}");
        }
    }

    public static void TestQueue()
    {
        var input = new System.Collections.Generic.List<int>(10000000);
        for (var i = 0; i < 10000000; i++) input.Add(Random.Shared.Next());

        var nativeQueue = new NativeCollection.Queue<int>();
        var managedQueue = new System.Collections.Generic.Queue<int>();

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) nativeQueue.Enqueue(value);

            while (nativeQueue.Count != 0) nativeQueue.Dequeue();
            stopwatch.Stop();
            Console.WriteLine($"native Queue : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) managedQueue.Enqueue(value);

            while (managedQueue.Count != 0) managedQueue.Dequeue();
            stopwatch.Stop();
            Console.WriteLine($"managed Queue : {stopwatch.ElapsedMilliseconds}");
        }
    }

    public static void TestSortedSet()
    {
        var input = new System.Collections.Generic.List<int>(100000);
        for (var i = 0; i < 100000; i++) input.Add(Random.Shared.Next());

        var nativeSortedSet = new NativeCollection.SortedSet<int>();
        var managedSortedSet = new System.Collections.Generic.SortedSet<int>();
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input) nativeSortedSet.Add(value);

                foreach (var value in input) nativeSortedSet.Remove(value);
            }
            
            stopwatch.Stop();
            Console.WriteLine($"native SortedSet : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input) managedSortedSet.Add(value);

                foreach (var value in input) managedSortedSet.Remove(value);
            }
            stopwatch.Stop();
            Console.WriteLine($"managed SortedSet : {stopwatch.ElapsedMilliseconds}");
        }
    }

    public unsafe static void TestSortedSetAddRemove()
    {
        var nativeSortedSet = new NativeCollection.SortedSet<int>();
        nativeSortedSet.Add(5);
        nativeSortedSet.Add(2);
        nativeSortedSet.Add(3);
        nativeSortedSet.Add(1);
        nativeSortedSet.Add(4);
        Console.WriteLine(nativeSortedSet.ToString());

        nativeSortedSet.Remove(2);
        nativeSortedSet.Remove(4);
        Console.WriteLine(nativeSortedSet.ToString());
    }

    public static unsafe void TestList()
    {
        
    }


    public static void TestMultiMap()
    {
        var input = new System.Collections.Generic.List<int>(100000);
        for (var i = 0; i < 100000; i++) input.Add(Random.Shared.Next());

        var managedMultiMap = new SortedDictionary<int, System.Collections.Generic.List<int>>();
        var nativeMultiMap = new NativeCollection.MultiMap<int, int>();

        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input)
                {
                    if (!managedMultiMap.TryGetValue(value, out var list))
                    {
                        list = new System.Collections.Generic.List<int>();
                        managedMultiMap.Add(value, list);
                    }

                    list.Add(1);
                }

                foreach (var value in input) managedMultiMap.Remove(value);
            }
            
            stopwatch.Stop();
            Console.WriteLine($"managedMultiMap : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input) nativeMultiMap.Add(value, 1);

                foreach (var value in input) nativeMultiMap.Remove(value);
            }

            stopwatch.Stop();
            Console.WriteLine($"nativeMultiMap : {stopwatch.ElapsedMilliseconds}");
        }
        
    }

    public unsafe static void TestHashSet()
    {
        var input = new System.Collections.Generic.List<int>(10000000);
        for (var i = 0; i < 10000000; i++)
        {
            input.Add(i);
        }

        NativeCollection.UnsafeType.HashSet<int>* nativeHashSet = NativeCollection.UnsafeType.HashSet<int>.Create();

        System.Collections.Generic.HashSet<int> managedHashSet = new System.Collections.Generic.HashSet<int>();
       
        
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input)
                {
                    nativeHashSet->Add(value);
                }
                foreach (var value in input)
                {
                    nativeHashSet->Remove(value);
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"nativeHashSet : {stopwatch.ElapsedMilliseconds}");
        }
        
        
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                foreach (var value in input)
                {
                    managedHashSet.Add(value);
                }
            
                foreach (var value in input)
                {
                    managedHashSet.Remove(value);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"managedHashSet : {stopwatch.ElapsedMilliseconds}");
        }
        
        
    }
}