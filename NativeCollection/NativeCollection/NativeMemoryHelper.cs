using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection;

public static unsafe class NativeMemoryHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Alloc(UIntPtr byteCount)
    {
        GC.AddMemoryPressure((long)byteCount);
#if NET6_0_OR_GREATER
        return NativeMemory.Alloc(byteCount);
#else
        return Marshal.AllocHGlobal((int)byteCount).ToPointer();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Alloc(UIntPtr elementCount, UIntPtr elementSize)
    {
        GC.AddMemoryPressure((long)(elementCount * elementSize));
#if NET6_0_OR_GREATER
        return NativeMemory.Alloc(elementCount, elementSize);
#else
        return Marshal.AllocHGlobal((int)(elementCount*elementSize)).ToPointer();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* AllocZeroed(UIntPtr byteCount)
    {
        GC.AddMemoryPressure((long)byteCount);
#if NET6_0_OR_GREATER
        return NativeMemory.AllocZeroed(byteCount);
#else
        var ptr = Marshal.AllocHGlobal((int)byteCount).ToPointer();
        Unsafe.InitBlockUnaligned(ptr,0,(uint)byteCount);
        return ptr;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* AllocZeroed(UIntPtr elementCount, UIntPtr elementSize)
    {
        GC.AddMemoryPressure((long)(elementCount * elementSize));
        
#if NET6_0_OR_GREATER
        return NativeMemory.AllocZeroed(elementCount, elementSize);
#else
        var ptr = Marshal.AllocHGlobal((int)(elementCount*elementSize)).ToPointer();
        Unsafe.InitBlockUnaligned(ptr,0,(uint)(elementCount*elementSize));
        return ptr;
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free<T>(T* ptr) where T : unmanaged
    {
#if NET6_0_OR_GREATER
        NativeMemory.Free(ptr);
#else
        Marshal.FreeHGlobal(new IntPtr(ptr));
#endif
    }
}