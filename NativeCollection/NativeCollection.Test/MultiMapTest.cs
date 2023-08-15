using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public class MultiMapTest
{
    [Fact]
    public void AddRemove()
    {
        MultiMap<int, int> multiMap = new MultiMap<int, int>();
        multiMap.Add(1,11);
        multiMap.Count.Should().Be(1);
        multiMap.Add(1,12);
        multiMap.Count.Should().Be(1);
        multiMap.Add(3,44);
        multiMap.Count.Should().Be(2);
        multiMap.Remove(3, 44).Should().Be(true);
        multiMap.Count.Should().Be(1);
        multiMap.Clear();
        multiMap.Count.Should().Be(0);
        multiMap.Remove(1).Should().Be(false);
        multiMap.Remove(1, 4).Should().Be(false);

        multiMap.Clear();
        for (int i = 0; i < 1000; i++)
        {
            multiMap.Add(1,Random.Shared.Next());
        }
        multiMap.Count.Should().Be(1);
    }

    [Fact]
    public void Enumerator()
    {
        SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
        MultiMap<int, int> multiMap = new MultiMap<int, int>();

        for (int i = 0; i < 1000; i++)
        {
            int value = Random.Shared.Next();
            sortedDictionary.Add(value,1);
            multiMap.Add(value,1);
        }

        var MultiMapEnumerator = multiMap.GetEnumerator();
        var sortedDictionaryEnumerator = sortedDictionary.GetEnumerator();
        while (sortedDictionaryEnumerator.MoveNext())
        {
            MultiMapEnumerator.MoveNext();
            int key = sortedDictionaryEnumerator.Current.Key;
            MultiMapEnumerator.Current.Key.Should().Be(key);
        }
    }
    
        
    [Fact]
    public void NativeCollectionClass()
    {
        MultiMap<int, int> multiMap = new MultiMap<int, int>();
        multiMap.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            multiMap.Add(Random.Shared.Next(),1);
        }
        multiMap.Dispose();
        multiMap.IsDisposed.Should().Be(true);
        multiMap.ReInit();
        multiMap.IsDisposed.Should().Be(false);
        multiMap.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            multiMap.Add(Random.Shared.Next(),1);
        }
    }
    
}