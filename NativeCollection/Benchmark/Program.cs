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
        //var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        //var summary = BenchmarkRunner.Run<BenchmarkMemoryPool>();


        NativeCollection.MultiMap<int, int> multiMap = new ();
        
        multiMap.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                multiMap.Add(i,j);
            }
        }
        multiMap.Clear();
        
        multiMap.Dispose();
    } 
    
}