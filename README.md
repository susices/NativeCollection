# NativeCollection
Native Collection library in c#

目标是实现一个常用集合的0gc高性能非托管库

已实现Stack Queue HashSet SortedSet list multimap.


下表是Multimap对比托管版本实现的性能测试 各项操作性能均快于托管版本一倍以上

```

BenchmarkDotNet v0.13.7, Windows 10 (10.0.19043.928/21H1/May2021Update)
Intel Core i7-9700 CPU 3.00GHz, 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.306
  [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2

```
|                           Method |       Categories | KeyCount |             Mean |          Error |         StdDev | Ratio | RatioSD |
|--------------------------------- |----------------- |--------- |-----------------:|---------------:|---------------:|------:|--------:|
|               **NativeAddAndRemove** |     **AddAndRemove** |    **10000** |    **237,981.43 ns** |     **410.339 ns** |     **383.831 ns** |  **0.44** |    **0.00** |
|              ManagedAddAndRemove |     AddAndRemove |    10000 |    546,403.44 ns |   1,182.838 ns |   1,048.555 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|               **NativeAddAndRemove** |     **AddAndRemove** |   **100000** |    **295,918.36 ns** |     **317.246 ns** |     **264.914 ns** |  **0.43** |    **0.00** |
|              ManagedAddAndRemove |     AddAndRemove |   100000 |    687,445.54 ns |     953.501 ns |     845.254 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|               **NativeAddAndRemove** |     **AddAndRemove** |  **1000000** |    **373,694.08 ns** |     **931.678 ns** |     **825.909 ns** |  **0.40** |    **0.00** |
|              ManagedAddAndRemove |     AddAndRemove |  1000000 |    945,009.41 ns |   1,869.211 ns |   1,657.007 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|               **NativeEnumerateAll** |     **EnumerateAll** |    **10000** |    **167,194.49 ns** |     **253.321 ns** |     **224.562 ns** |  **0.86** |    **0.00** |
|              ManagedEnumerateAll |     EnumerateAll |    10000 |    194,746.90 ns |   1,213.956 ns |   1,135.535 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|               **NativeEnumerateAll** |     **EnumerateAll** |   **100000** |  **3,032,551.02 ns** |  **67,905.637 ns** | **199,155.478 ns** |  **1.10** |    **0.08** |
|              ManagedEnumerateAll |     EnumerateAll |   100000 |  2,778,049.31 ns |  55,453.630 ns | 154,582.555 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|               **NativeEnumerateAll** |     **EnumerateAll** |  **1000000** | **36,404,652.20 ns** | **171,751.129 ns** | **143,419.971 ns** |  **1.09** |    **0.01** |
|              ManagedEnumerateAll |     EnumerateAll |  1000000 | 33,500,559.56 ns | 185,616.319 ns | 173,625.620 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|  **NativeEnumerateAndRemoveFirst10** | **EnumerateFirst10** |    **10000** |         **26.25 ns** |       **0.032 ns** |       **0.027 ns** |  **0.36** |    **0.00** |
| ManagedEnumerateAndRemoveFirst10 | EnumerateFirst10 |    10000 |         73.63 ns |       0.388 ns |       0.344 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|  **NativeEnumerateAndRemoveFirst10** | **EnumerateFirst10** |   **100000** |         **25.40 ns** |       **0.081 ns** |       **0.071 ns** |  **0.33** |    **0.00** |
| ManagedEnumerateAndRemoveFirst10 | EnumerateFirst10 |   100000 |         76.37 ns |       0.116 ns |       0.097 ns |  1.00 |    0.00 |
|                                  |                  |          |                  |                |                |       |         |
|  **NativeEnumerateAndRemoveFirst10** | **EnumerateFirst10** |  **1000000** |         **25.63 ns** |       **0.038 ns** |       **0.035 ns** |  **0.32** |    **0.00** |
| ManagedEnumerateAndRemoveFirst10 | EnumerateFirst10 |  1000000 |         79.89 ns |       0.439 ns |       0.390 ns |  1.00 |    0.00 |
