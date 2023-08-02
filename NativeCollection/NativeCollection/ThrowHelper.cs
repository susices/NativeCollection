using System.Diagnostics.CodeAnalysis;

namespace NativeCollection;

public static class ThrowHelper
{
    [DoesNotReturn]
    public static void StackInitialCapacityException()
    {
        throw new ArgumentOutOfRangeException();
    }
}