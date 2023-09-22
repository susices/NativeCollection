// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using NativeCollection;
using NativeCollection.UnsafeType;

public static class Program
{
    public unsafe static void Main()
    {
        //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        //var summary = BenchmarkRunner.Run<BenchmarkSortedSet>();
        
        RunAll().Wait();
    }

    public static async Task RunAll()
    {
        int index = 0;
        while (true)
        {
            Task[] tasks = new[] { Run(), Run(), Run(), Run(), Run(), Run(), Run(), Run(), Run(), Run() };
            //Task[] tasks = new[] { Run() };
            await Task.WhenAll(tasks);
            Console.WriteLine($"index:{index++}");
        }
    }


    public static async Task Run()
    {
        await Task.Run((() =>
        {
            NativeCollection.MultiMap<int, int> multiMap = new NativeCollection.MultiMap<int, int>();
            for (int i = 0; i < 100; i++)
            {
                multiMap.Add(i,i);
            }

            for (int i = 0; i < 100; i++)
            {
                multiMap.Remove(i,i);
            }
            //Console.WriteLine($"threadID:{Environment.CurrentManagedThreadId}");
        }));
        //await Task.CompletedTask;
        
    }
    
}