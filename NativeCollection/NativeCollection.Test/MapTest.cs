using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public class MapTest
{
    [Fact]
    public void AddRemove()
    {
        Map<int, int> map = new Map<int, int>();
        map.Add(1,11);
        map.Count.Should().Be(1);
        map[1].Should().Be(11);
        map.Add(1,12);
        map.Count.Should().Be(1);
        map[1].Should().Be(11);
        map.Add(3,44);
        map.Count.Should().Be(2);
        map.Remove(3).Should().Be(true);
        map.Count.Should().Be(1);
        map.Clear();
        map.Count.Should().Be(0);
        map.Remove(1).Should().Be(false);
        map.Clear();
        for (int i = 0; i < 1000; i++)
        {
            map.Add(1,Random.Shared.Next());
        }
        map.Count.Should().Be(1);
     
        map.Clear();
        map.Add(1,100);
        map[1].Should().Be(100);
        map[1] = 101;
        map[1].Should().Be(101);
        map[2] = 102;
        map[2].Should().Be(102);
        SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
        sortedDictionary.Add(1,100);
        sortedDictionary[1] = 101;
        sortedDictionary[1].Should().Be(101);
    }
    
    [Fact]
    public void Enumerator()
    {
        SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
        Map<int, int> map = new Map<int, int>();

        for (int i = 0; i < 1000; i++)
        {
            int value = Random.Shared.Next();
            sortedDictionary.Add(value,1);
            map.Add(value,1);
        }

        using var mapEnumerator = map.GetEnumerator();
        using var sortedDictionaryEnumerator = sortedDictionary.GetEnumerator();
        while (sortedDictionaryEnumerator.MoveNext())
        {
            mapEnumerator.MoveNext();
            int key = sortedDictionaryEnumerator.Current.Key;
            mapEnumerator.Current.Key.Should().Be(key);
        }
    }
    
    [Fact]
    public void NativeCollectionClass()
    {
        Map<int, int> map = new Map<int, int>();
        map.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            map.Add(Random.Shared.Next(),1);
        }
        map.Dispose();
        map.IsDisposed.Should().Be(true);
        map.ReInit();
        map.IsDisposed.Should().Be(false);
        map.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            map.Add(Random.Shared.Next(),1);
        }
    }
    
}