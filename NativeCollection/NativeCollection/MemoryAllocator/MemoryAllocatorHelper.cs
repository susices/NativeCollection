using System.Runtime.CompilerServices;

namespace NativeCollection
{
    public static class MemoryAllocatorHelper
    {
        /// <summary>
        /// 转换为8的倍数
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RoundTo(uint value, uint roundNum)
        {
            return (value + (roundNum-1)) & ~(roundNum-1); 
        }
    }
}

