using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace NativeCollection.Test;

public class HashSetTest
{
    [Fact]
    public void AddRemove()
    {
        HashSet<int> hashSet = new HashSet<int>();
        hashSet.Add(1);
        hashSet.Add(2);
        hashSet.Add(1);
        hashSet.Count.Should().Be(2);
        hashSet.Remove(2);
        hashSet.Count.Should().Be(1);
        hashSet.Remove(1);
        hashSet.Count.Should().Be(0);
        hashSet.Add(23);
        hashSet.Add(123);
        hashSet.Clear();
        hashSet.Count.Should().Be(0);
        hashSet.Remove(1).Should().Be(false);
        hashSet.Add(1);
        hashSet.Remove(1).Should().Be(true);
    }

    [Fact]
    public void Contains()
    {
        HashSet<int> hashSet = new HashSet<int>();
        hashSet.Contains(1).Should().Be(false);
        hashSet.Add(1);
        hashSet.Contains(1).Should().Be(true);
        hashSet.Contains(2).Should().Be(false);
        hashSet.Clear();
        hashSet.Contains(1).Should().Be(false);
    }
    
    [Fact]
    public void AddRemoveInEnumerator()
    {
        HashSet<int> hashSet = new HashSet<int>();
        
        for (int i = 0; i < 100; i++)
        {
            int value = i;
            hashSet.Add(value);
        }

        foreach (var value in hashSet)
        {
            if (value%2==0)
            {
                hashSet.Remove(value);
            }
        }

        hashSet.Count.Should().Be(50);
        hashSet.Clear();
        
        for (int i = 0; i < 100; i++)
        {
            int value = Random.Shared.Next();
            hashSet.Add(value);
        }

        bool hasException = false;

        try
        {
            foreach (var value in hashSet)
            {
                hashSet.Add(Random.Shared.Next());
            }
        }
        catch (Exception e)
        {
            hasException = true;
        }

        hasException.Should().Be(true);
        hasException = false;
        
        
    }
    
    [Fact]
    public void NativeCollectionClass()
    {
        HashSet<int> hashSet = new HashSet<int>();
        hashSet.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            hashSet.Add(Random.Shared.Next());
        }
        hashSet.Dispose();
        hashSet.IsDisposed.Should().Be(true);
        hashSet.ReInit();
        hashSet.IsDisposed.Should().Be(false);
        hashSet.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            hashSet.Add(Random.Shared.Next());
        }
    }
}