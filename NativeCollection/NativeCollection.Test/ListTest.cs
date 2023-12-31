using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public class ListTest
{
    [Fact]
    public void AddRemove()
    {
        List<int> nativeList = new List<int>();
        System.Collections.Generic.List<int> managedList = new System.Collections.Generic.List<int>();

        nativeList.Remove(1).Should().Be(false);
        
        nativeList.Add(1);
        nativeList.Remove(1).Should().Be(true);
        int count = 1000;

        for (int i = 0; i < count; i++)
        {
            int value = Random.Shared.Next();
            nativeList.Add(value);
            managedList.Add(value);
            nativeList.Count.Should().Be(i + 1);
            nativeList.Contains(value).Should().Be(true);
        }

        for (int i = count-1; i >= 0; i--)
        {
            nativeList[i].Should().Be(managedList[i]);

            int value = nativeList[i];
            nativeList.Remove(value);
            managedList.Remove(value);
            nativeList.Count.Should().Be(managedList.Count);
        }

        nativeList.Count.Should().Be(0);

    }
    
    [Fact]
    public void RemoveAt()
    {
        List<int> nativeList = new List<int>();
        for (int i = 0; i < 1000; i++)
        {
            nativeList.Add(i);
        }
        
        nativeList.RemoveAt(10);
        nativeList.Count.Should().Be(999);
        nativeList[10].Should().Be(11);
     
        nativeList.RemoveAt(998);
        nativeList.Count.Should().Be(998);
        nativeList[997].Should().Be(998);
        
        nativeList.Clear();

        for (int i = -100; i < 100; i++)
        {
            bool hasException = false;
            try
            {
                nativeList.RemoveAt(0);
            }
            catch (Exception e)
            {
                hasException = true;
            }

            hasException.Should().Be(true);
        }
    }

    [Fact]
    public void Insert()
    {
        List<int> list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        list.Insert(1,10);
        list[0].Should().Be(1);
        list[1].Should().Be(10);
        list[2].Should().Be(2);
        list[3].Should().Be(3);
        list.Count.Should().Be(4);
    }

    [Fact]
    public void InsertRange()
    {
        List<int> list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(4);
        list.Add(5);
        Span<int> range = stackalloc int[] { 11, 12, 13 };
        list.InsertRange(2,range);

        list[0].Should().Be(1);
        list[1].Should().Be(2);
        list[2].Should().Be(11);
        list[3].Should().Be(12);
        list[4].Should().Be(13);
        list[5].Should().Be(3);
        list[6].Should().Be(4);
        list[7].Should().Be(5);
        list.Count.Should().Be(8);
    }

    [Fact]
    public void RemoveRange()
    {
        List<int> list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(4);
        list.Add(5);
        
        list.RemoveRange(1,2);
        list[0].Should().Be(1);
        list[1].Should().Be(4);
        list[2].Should().Be(5);
        list.Count.Should().Be(3);
    }


    [Fact]
    public void Enumerator()
    {
        List<int> nativeList = new List<int>();
        System.Collections.Generic.List<int> managedList = new System.Collections.Generic.List<int>();

        for (int i = 0; i < 10000; i++)
        {
            int value = Random.Shared.Next();
            nativeList.Add(value);
            managedList.Add(value);
        }

        var nativeEnumerator = nativeList.GetEnumerator();
        var managedEnumerator = managedList.GetEnumerator();
        while (managedEnumerator.MoveNext())
        {
            nativeEnumerator.MoveNext();
            nativeEnumerator.Current.Should().Be(managedEnumerator.Current);
        }
    }


    [Fact]
    public void Contains()
    {
        List<int> nativeList = new List<int>();
        System.Collections.Generic.List<int> managedList = new System.Collections.Generic.List<int>();
        
        for (int i = 0; i < 10000; i++)
        {
            int value = Random.Shared.Next(0,10000);
            nativeList.Add(value);
            managedList.Add(value);
        }
        
        for (int i = 0; i < 10000; i++)
        {
            int value = Random.Shared.Next(0,10000);
            nativeList.Contains(value).Should().Be(managedList.Contains(value));
        }
    }


    [Fact]
    public void IndexOf()
    {
        List<int> nativeList = new List<int>();
        for (int i = 0; i < 1000; i++)
        {
            nativeList.Add(i);
        }

        for (int i = 0; i < 1000; i++)
        {
            nativeList.IndexOf(i).Should().Be(i);
        }
    }

    [Fact]
    public void Clear()
    {
        List<int> nativeList = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < Random.Shared.Next(1,9999); j++)
            {
                nativeList.Add(Random.Shared.Next());
            }
            nativeList.Clear();
            nativeList.Count.Should().Be(0);
        }
    }

    [Fact]
    public void RefIndex()
    {
        List<int> nativeList = new List<int>();
        nativeList.Add(123);
        nativeList.Add(456);
        nativeList[0] = 789;
        nativeList[0].Should().Be(789);

        ref var value = ref nativeList[0];
        value = 999;
        nativeList[0].Should().Be(999);

        nativeList[0] = 0;
        value.Should().Be(0);
    }

    [Fact]
    public void CopyTo()
    {
        int[] array = new int[100];
        List<int> list = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            list.Add(i);
        }

        list.CopyTo(array,0);

        for (int i = 0; i < 100; i++)
        {
            array[i].Should().Be(i);
        }

    }

    [Fact]
    public void NativeCollectionClass()
    {
        List<int> nativeList = new List<int>();
        nativeList.IsDisposed.Should().Be(false);
        for (int i = 0; i < 100; i++)
        {
            nativeList.Add(Random.Shared.Next());
        }
        nativeList.Dispose();
        nativeList.IsDisposed.Should().Be(true);
        nativeList.ReInit();
        nativeList.IsDisposed.Should().Be(false);
        nativeList.Count.Should().Be(0);
        for (int i = 0; i < 100; i++)
        {
            nativeList.Add(Random.Shared.Next());
        }
    }
}