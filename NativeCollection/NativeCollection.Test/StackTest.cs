using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public class StackTest
{
    [Fact]
    public void PushPopClear()
    {
        Stack<int> nativeStack = new Stack<int>();
        System.Collections.Generic.Stack<int> managedStack = new System.Collections.Generic.Stack<int>();

        for (int i = 0; i < 10; i++)
        {
            int count = Random.Shared.Next(1, 1000);
            for (int j = 0; j < count; j++)
            {
                int value = Random.Shared.Next();
                nativeStack.Push(value);
                managedStack.Push(value);
            }

            for (int j = 0; j < count; j++)
            {
                nativeStack.Pop().Should().Be(managedStack.Pop());
            }
            managedStack.Clear();
            nativeStack.Clear();
            nativeStack.Count.Should().Be(0);
        }
        nativeStack.Clear();
        nativeStack.Count.Should().Be(0);
        nativeStack.Clear();
        nativeStack.Count.Should().Be(0);
    }
    
    
    [Fact]
    public void Contains()
    {
        Stack<int> nativeStack = new Stack<int>();
        System.Collections.Generic.Stack<int> managedStack = new System.Collections.Generic.Stack<int>();

        int count = 1000;
        for (int j = 0; j < count; j++)
        {
            int value = Random.Shared.Next(1,99999);
            nativeStack.Push(value);
            managedStack.Push(value);
            nativeStack.Contains(value).Should().Be(true);
        }

        while (managedStack.TryPop(out var popValue))
        {
            nativeStack.Contains(popValue).Should().Be(true);
        }

        for (int i = 0; i < 1000; i++)
        {
            nativeStack.Contains(Random.Shared.Next(-100000, 0)).Should().Be(false);
        }
    }
    
    [Fact]
    public void Peak()
    {
        Stack<int> nativeStack = new Stack<int>();
        System.Collections.Generic.Stack<int> managedStack = new System.Collections.Generic.Stack<int>();

        for (int i = 0; i < 10; i++)
        {
            int count = Random.Shared.Next(1, 1000);
            for (int j = 0; j < count; j++)
            {
                int value = Random.Shared.Next();
                nativeStack.Push(value);
                managedStack.Push(value);
                nativeStack.Peak().Should().Be(managedStack.Peek());
            }

            for (int j = 0; j < count; j++)
            {
                nativeStack.Peak().Should().Be(managedStack.Peek());
                nativeStack.Pop();
                managedStack.Pop();
            }

            nativeStack.TryPop(out _).Should().Be(false);
            bool hasException = false;
            try
            {
                nativeStack.Pop();
            }
            catch (Exception e)
            {
                hasException = true;
            }

            hasException.Should().Be(true);
            
            
            managedStack.Clear();
            nativeStack.Clear();
        }
        
    }
    
    [Fact]
    public void NativeCollectionClass()
    {
        Stack<int> stack = new Stack<int>();
        stack.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            stack.Push(Random.Shared.Next());
        }
        stack.Dispose();
        stack.IsDisposed.Should().Be(true);
        stack.ReInit();
        stack.IsDisposed.Should().Be(false);
        stack.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            stack.Push(Random.Shared.Next());
        }
    }
}