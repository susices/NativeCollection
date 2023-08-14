// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using NativeCollection;

public static class Program
{
    public static void Main()
    {
        // TestStack();
        //TestQueue();
        //TestSortedSet();
        //TestSortedSetAddRemove();
        //TestList();
        TestMultiMap();
    }

    public static void TestStack()
    {
        var input = new List<int>(10000000);
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
        var input = new List<int>(10000000);
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
        var input = new List<int>(1000000);
        for (var i = 0; i < 1000000; i++) input.Add(Random.Shared.Next());

        var nativeSortedSet = new NativeCollection.SortedSet<int>();
        var managedSortedSet = new System.Collections.Generic.SortedSet<int>();
        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) nativeSortedSet.Add(value);

            foreach (var value in input) nativeSortedSet.Remove(value);
            stopwatch.Stop();
            Console.WriteLine($"native SortedSet : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) managedSortedSet.Add(value);

            foreach (var value in input) managedSortedSet.Remove(value);
            stopwatch.Stop();
            Console.WriteLine($"managed SortedSet : {stopwatch.ElapsedMilliseconds}");
        }
    }

    public static void TestSortedSetAddRemove()
    {
        var nativeSortedSet = new NativeCollection.SortedSet<int>();
        nativeSortedSet.Add(1);
        nativeSortedSet.Add(2);
        nativeSortedSet.Add(3);
        nativeSortedSet.Add(4);
        nativeSortedSet.Add(5);
        Console.WriteLine(nativeSortedSet);

        nativeSortedSet.Remove(2);
        nativeSortedSet.Remove(4);
        Console.WriteLine(nativeSortedSet);
    }

    public static unsafe void TestList()
    {
        var list = NativeCollection.Internal.List<int>.Create(10);
        list->Add(1);
        list->Add(2);
        list->Add(3);
        list->Add(4);
        list->Add(5);
        list->Add(6);
        list->Add(7);
        list->Add(8);
        list->Add(9);
        list->Add(10);
        list->Add(11);
        list->Add(12);
        list->Add(13);
        list->Add(14);
        list->Add(15);
        Console.WriteLine((*list).ToString());

        list->Remove(1);
        // list->Remove(3);
        // list->Remove(5);
        // list->Remove(7);
        Console.WriteLine((*list).ToString());
    }


    public static void TestMultiMap()
    {
        var input = new List<int>(1000000);
        for (var i = 0; i < 1000000; i++) input.Add(Random.Shared.Next());

        var managedMultiMap = new SortedDictionary<int, List<int>>();
        var nativeMultiMap = new MultiMap<int, int>();

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                if (!managedMultiMap.TryGetValue(value, out var list))
                {
                    list = new List<int>();
                    managedMultiMap.Add(value, list);
                }

                list.Add(1);
            }

            foreach (var value in input) managedMultiMap.Remove(value);
            stopwatch.Stop();
            Console.WriteLine($"managedMultiMap : {stopwatch.ElapsedMilliseconds}");
        }

        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var value in input) nativeMultiMap.Add(value, 1);

            foreach (var value in input) nativeMultiMap.Remove(value);
            stopwatch.Stop();
            Console.WriteLine($"nativeMultiMap : {stopwatch.ElapsedMilliseconds}");
        }
    }
}