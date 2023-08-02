# NativeCollection
Native Collection library in c#

目标是实现一个常用集合的非托管库

已实现Stack Queue SortedSet.

如下为非托管与标准库性能测试 数值越小越好

native stack : 73

managed stack : 87

native Queue : 69

managed Queue : 116

native SortedSet : 1490

managed SortedSet : 1675
