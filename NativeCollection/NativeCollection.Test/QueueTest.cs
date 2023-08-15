using FluentAssertions;
using Xunit;

namespace NativeCollection.Test.SortedSet;

public class QueueTest
{
    [Fact]
    public void EnqueueDequeue()
    {
        System.Collections.Generic.Queue<int> managedQueue = new System.Collections.Generic.Queue<int>();
        Queue<int> nativeQueue = new Queue<int>();

        for (int i = 0; i < 1000; i++)
        {
            int value = Random.Shared.Next();
            managedQueue.Enqueue(value);
            nativeQueue.Enqueue(value);
            nativeQueue.Count.Should().Be(i + 1);
        }

        for (int i = 0; i < 1000; i++)
        {
            managedQueue.Dequeue().Should().Be(nativeQueue.Dequeue());
            nativeQueue.Count.Should().Be(1000 - i - 1);
        }

        bool hasException = false;

        try
        {
            nativeQueue.Dequeue();
        }
        catch (Exception e)
        {
            hasException = true;
        }

        hasException.Should().Be(true);
    }
    
    [Fact]
    public void Peak()
    {
        Queue<int> nativeQueue = new Queue<int>();
        System.Collections.Generic.Queue<int> managedQueue = new System.Collections.Generic.Queue<int>();

        for (int i = 0; i < 1000; i++)
        {
            int value = Random.Shared.Next();
            managedQueue.Enqueue(value);
            nativeQueue.Enqueue(value);
            nativeQueue.Peek().Should().Be(managedQueue.Peek());
            nativeQueue.TryPeek(out var peakValue);
            managedQueue.TryPeek(out var peakValue2);
            peakValue.Should().Be(peakValue2);
        }

        {
            nativeQueue.Clear();
            nativeQueue.TryPeek(out var value).Should().Be(false);
        }
        
        
    }
    
    [Fact]
    public void Clear()
    {
        Queue<int> nativeQueue = new Queue<int>();
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < Random.Shared.Next(1,100); j++)
            {
                int value = Random.Shared.Next();
                nativeQueue.Enqueue(value);
            }
            nativeQueue.Clear();
            nativeQueue.Count.Should().Be(0);
        }
        
    }
}