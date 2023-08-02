// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Program
{
    public static  void Main()
    {
        Console.WriteLine("hello");
        TestStack();
        TestQueue();
        TestSortedSet();
    }


    public static void TestStack()
    {
        List<int> input = new List<int>(10000000);
        for (int i = 0; i < 10000000; i++)
        {
            input.Add(Random.Shared.Next());
        }

        NativeCollection.Stack<int> nativeStack = new NativeCollection.Stack<int>();
        Stack<int> managedStack = new Stack<int>();

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
        Queue<int> managedQueue = new Queue<int>();

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
        SortedSet<int> managedSortedSet = new SortedSet<int>();
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

}