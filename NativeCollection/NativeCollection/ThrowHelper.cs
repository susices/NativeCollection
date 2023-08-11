using System.Diagnostics.CodeAnalysis;

namespace NativeCollection;

public static class ThrowHelper
{
    [DoesNotReturn]
    public static void StackInitialCapacityException()
    {
        throw new ArgumentOutOfRangeException();
    }

    [DoesNotReturn]
    public static void StackEmptyException()
    {
        throw new InvalidOperationException("Stack Empty");
    }

    [DoesNotReturn]
    public static void QueueEmptyException()
    {
        throw new InvalidOperationException("EmptyQueue");
    }
    
    [DoesNotReturn]
    public static void ListInitialCapacityException()
    {
        throw new ArgumentOutOfRangeException();
    }

    [DoesNotReturn]
    public static void IndexMustBeLessException()
    {
        throw new ArgumentOutOfRangeException("IndexMustBeLess");
    }
    
    
    [DoesNotReturn]
    public static void ListSmallCapacity()
    {
        throw new ArgumentOutOfRangeException("SmallCapacity");
    }
    
   
    
    
}