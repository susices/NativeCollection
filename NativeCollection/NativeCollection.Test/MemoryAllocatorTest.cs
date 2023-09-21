using FluentAssertions;
using Xunit;

namespace NativeCollection.Test;

public unsafe class MemoryAllocatorTest
{
    
    [Fact]
    public void AllocFreeSingleThread()
    {
        MemoryAllocator.Init(8,8*1024, usedInMultiThread:false);
        var ptr = MemoryAllocator.Alloc(16);
        Assert.True(ptr!=null);
        MemoryAllocator.SizeOf(ptr).Should().Be(16);
        MemoryAllocator.Free(ptr);

        for (uint i = 8; i < 100000; i+=8)
        {
            var newPtr = MemoryAllocator.Alloc(i);
           
            MemoryAllocator.Free(newPtr);
        }
    }
    
    
    [Fact]
    public void AllocFreeMultiThread()
    {
       MemoryAllocator.Init(8,8*1024);
       var ptr = MemoryAllocator.Alloc(16);
       Assert.True(ptr!=null);
       MemoryAllocator.SizeOf(ptr).Should().Be(16);
       MemoryAllocator.Free(ptr);

       for (uint i = 8; i < 100000; i+=8)
       {
           var newPtr = MemoryAllocator.Alloc(i);
           MemoryAllocator.Free(newPtr);
       }
    }
}