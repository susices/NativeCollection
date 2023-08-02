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
}