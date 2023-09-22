using FluentAssertions;
using NativeCollection;
using Xunit;

namespace MemoryProfile;

public class MemoryLeakTest
{
    [Fact]
    public void ListMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

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
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void QueueMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

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
        
        var memory = MemoryAllocator.GetUsedMemorySize();
           memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void StackMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

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
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void SortedSetMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

        NativeCollection.SortedSet<int> sortedSet = new NativeCollection.SortedSet<int>();
        
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                sortedSet.Add(i);
            }
        }
        //sortedSet.Clear();
        
        sortedSet.Dispose();
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void MultiMapMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();
       
        MultiMap<int, int> multiMap = new (1000);
        
        multiMap.Clear();
        
        for (int i = 0; i < 129; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                multiMap.Add(i,j);
            }
        }
        multiMap.Clear();
        
        multiMap.Dispose();
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
        
        multiMap.ReInit();
        
         for (int i = 0; i < 100; i++)
         {
             for (int j = 0; j < 10; j++)
             {
                 multiMap.Add(i,j);
             }
         }
         multiMap.Clear();
         multiMap.Dispose();
         memory = MemoryAllocator.GetUsedMemorySize();
         memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void MapMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();
       
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
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void HashSetMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

        NativeCollection.HashSet<int> hashSet = new ();
        
        hashSet.Clear();
        
        for (int i = 0; i < 1000; i++)
        {
            hashSet.Add(i);
            
        }
        hashSet.Clear();
        
        hashSet.Dispose();
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
    
    [Fact]
    public void UnOrderMapMemoryLeak()
    {
        MemoryAllocator.TryDefaultInit();
        var initMemory = MemoryAllocator.GetUsedMemorySize();

        NativeCollection.UnOrderMap<int,int> unOrderMap = new ();
        
        unOrderMap.Clear();
        
        for (int i = 0; i < 10000; i++)
        {
            unOrderMap.Add(i,1);
        }
        for (int i = 0; i < 10000; i++)
        {
            unOrderMap.Remove(i);
        }
        
        unOrderMap.Clear();
        
        unOrderMap.Dispose();
        
        var memory = MemoryAllocator.GetUsedMemorySize();
        memory.Should().Be(initMemory);
    }
}