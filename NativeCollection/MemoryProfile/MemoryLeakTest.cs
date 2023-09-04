using FluentAssertions;
using NativeCollection;
using Xunit;

namespace MemoryProfile;

public class MemoryLeakTest
{
    [Fact]
    public void ListMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.List<int> list = new NativeCollection.List<int>();
        
        list.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                list.Add(i);
            }
            for (int j = 0; j < 10; j++)
            {
                list.Remove(i);
            }
        }
        list.Clear();
        
        list.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void QueueMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.Queue<int> queue = new NativeCollection.Queue<int>();
        
        queue.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                queue.Enqueue(i);
            }
            for (int j = 0; j < 10; j++)
            {
                queue.Dequeue();
            }
        }
        queue.Clear();
        
        queue.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void StackMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.Stack<int> stack = new NativeCollection.Stack<int>();
        
        stack.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                stack.Push(i);
            }
            for (int j = 0; j < 10; j++)
            {
                stack.Pop();
            }
        }
        stack.Clear();
        
        stack.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void SortedSetMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.SortedSet<int> sortedSet = new NativeCollection.SortedSet<int>();
        
        sortedSet.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                sortedSet.Add(i);
            }

        }
        sortedSet.Clear();
        
        sortedSet.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void MultiMapMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();
       
        MultiMap<int, int> multiMap = new ();
        
        multiMap.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                multiMap.Add(i,j);
            }
        }
        //multiMap.Clear();
        
        multiMap.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void MapMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();
       
        Map<int, int> map = new ();
        
        map.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                map.Add(i,j);
            }
        }
        map.Clear();
        
        map.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void HashSetMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.HashSet<int> hashSet = new ();
        
        hashSet.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                hashSet.Add(i);
            }
            for (int j = 0; j < 10; j++)
            {
                hashSet.Remove(i);
            }
        }
        hashSet.Clear();
        
        hashSet.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void UnOrderMapMemoryLeak()
    {
        var initMemory = NativeMemoryHelper.GetNativeMemoryBytes();

        NativeCollection.UnOrderMap<int,int> unOrderMap = new ();
        
        unOrderMap.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            unOrderMap.Add(i,1);
            
        }
        for (int i = 0; i < 100; i++)
        {
            unOrderMap.Remove(i);
        }
        
        unOrderMap.Clear();
        
        unOrderMap.Dispose();
        
        var memory = NativeMemoryHelper.GetNativeMemoryBytes();
        memory.Should().Be(initMemory);
    }
}