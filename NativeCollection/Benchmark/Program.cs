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
        var summary = BenchmarkRunner.Run<BenchmarkSortedSet>();
    } 
    
}