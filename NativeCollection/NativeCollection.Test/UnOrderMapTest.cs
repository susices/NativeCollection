using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public class UnOrderMapTest
{
    [Fact]
    public void AddRemove()
    {
        UnOrderMap<int, int> unOrderMap = new UnOrderMap<int, int>();
        unOrderMap.Add(1,11);
        unOrderMap.Count.Should().Be(1);
        unOrderMap[1].Should().Be(11);
        unOrderMap.Count.Should().Be(1);
        unOrderMap[1].Should().Be(11);
        unOrderMap.Add(3,44);
        unOrderMap.Count.Should().Be(2);
        unOrderMap.Remove(3).Should().Be(true);
        unOrderMap.Count.Should().Be(1);
        unOrderMap.Clear();
        unOrderMap.Count.Should().Be(0);
        unOrderMap.Remove(1).Should().Be(false);
        unOrderMap.Clear();
     
        unOrderMap.Clear();
        unOrderMap.Add(1,100);
        unOrderMap[1].Should().Be(100);
        unOrderMap[1] = 101;
        unOrderMap[1].Should().Be(101);
        unOrderMap[2] = 102;
        unOrderMap[2].Should().Be(102);
    }
    
    [Fact]
    public void Enumerator()
    {
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        UnOrderMap<int, int> unOrderMap = new UnOrderMap<int, int>();

        for (int i = 0; i < 1000; i++)
        {
            int value = Random.Shared.Next();
            dictionary.Add(value,1);
            unOrderMap.Add(value,1);
        }

        using var mapEnumerator = unOrderMap.GetEnumerator();
        using var sortedDictionaryEnumerator = dictionary.GetEnumerator();
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
        UnOrderMap<int, int> unOrderMap = new UnOrderMap<int, int>();
        unOrderMap.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            unOrderMap.Add(Random.Shared.Next(),1);
        }
        unOrderMap.Dispose();
        unOrderMap.IsDisposed.Should().Be(true);
        unOrderMap.ReInit();
        unOrderMap.IsDisposed.Should().Be(false);
        unOrderMap.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            unOrderMap.Add(Random.Shared.Next(),1);
        }
    }
}