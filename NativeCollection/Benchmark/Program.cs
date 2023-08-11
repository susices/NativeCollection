// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NativeCollection;

public static class Program
{
    public static  void Main()
    {
        // TestStack();
        // TestQueue();
        // TestSortedSet();
        //TestSortedSetAddRemove();
        //TestList();
        TestMultiMap();
    }


    public static void TestStack()
    {
        List<int> input = new List<int>(10000000);
        for (int i = 0; i < 10000000; i++)
        {
            input.Add(Random.Shared.Next());
        }

        NativeCollection.Stack<int> nativeStack = new NativeCollection.Stack<int>();
        System.Collections.Generic.Stack<int> managedStack = new System.Collections.Generic.Stack<int>();

        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                nativeStack.Push(value);
            }

            while (nativeStack.Count!=0)
            {
                nativeStack.Pop();
            }
            stopwatch.Stop();
            Console.WriteLine($"native stack : {stopwatch.ElapsedMilliseconds}");
        }
        
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                managedStack.Push(value);
            }

            while (managedStack.Count!=0)
            {
                managedStack.Pop();
            }
            stopwatch.Stop();
            Console.WriteLine($"managed stack : {stopwatch.ElapsedMilliseconds}");
        }
    }
    
    public static void TestQueue()
    {
        List<int> input = new List<int>(10000000);
        for (int i = 0; i < 10000000; i++)
        {
            input.Add(Random.Shared.Next());
        }

        NativeCollection.Queue<int> nativeQueue = new NativeCollection.Queue<int>();
        System.Collections.Generic.Queue<int> managedQueue = new System.Collections.Generic.Queue<int>();

        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                nativeQueue.Enqueue(value);
            }

            while (nativeQueue.Count!=0)
            {
                nativeQueue.Dequeue();
            }
            stopwatch.Stop();
            Console.WriteLine($"native Queue : {stopwatch.ElapsedMilliseconds}");
        }
        
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                managedQueue.Enqueue(value);
            }

            while (managedQueue.Count!=0)
            {
                managedQueue.Dequeue();
            }
            stopwatch.Stop();
            Console.WriteLine($"managed Queue : {stopwatch.ElapsedMilliseconds}");
        }
    }

    public static void TestSortedSet()
    {
        List<int> input = new List<int>(1000000);
        for (int i = 0; i < 1000000; i++)
        {
            input.Add(Random.Shared.Next());
        }

        NativeCollection.SortedSet<int> nativeSortedSet = new NativeCollection.SortedSet<int>();
        System.Collections.Generic.SortedSet<int> managedSortedSet = new System.Collections.Generic.SortedSet<int>();
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                nativeSortedSet.Add(value);
            }

            foreach (var value in input)
            {
                nativeSortedSet.Remove(value);
            }
            stopwatch.Stop();
            Console.WriteLine($"native SortedSet : {stopwatch.ElapsedMilliseconds}");
        }
        
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var value in input)
            {
                managedSortedSet.Add(value);
            }

            foreach (var value in input)
            {
                managedSortedSet.Remove(value);
            }
            stopwatch.Stop();
            Console.WriteLine($"managed SortedSet : {stopwatch.ElapsedMilliseconds}");
            
        }
    }

    public static void TestSortedSetAddRemove()
    {
        NativeCollection.SortedSet<int> nativeSortedSet = new NativeCollection.SortedSet<int>();
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
        NativeCollection.Internal.List<int>* list = NativeCollection.Internal.List<int>.Create(10);
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


    public unsafe static void TestMultiMap()
    {
        MultiMap<int, int> multiMap = new MultiMap<int, int>();
        
        multiMap.Add(3,9);
        multiMap.Add(3,7);
        multiMap.Add(3,8);
        
        multiMap.Add(1,3);
        multiMap.Add(1,2);
        multiMap.Add(1,1);
        
        multiMap.Add(5,1);
        multiMap.Add(5,13);
        multiMap.Add(5,5);

        foreach (MultiMapPair<int, int> pair in multiMap)
        {
            Console.WriteLine($"key:{pair.Key}");
            Console.WriteLine($"list count:{pair.Value.Count}");
           
            foreach (int value in *pair._value)
            {
                Console.WriteLine($"value:{value}");
            }
        }
    }

}