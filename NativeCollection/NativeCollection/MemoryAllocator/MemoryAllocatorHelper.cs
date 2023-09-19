namespace NativeCollection
{
    public static class MemoryAllocatorHelper
    {
        /// <summary>
        /// 转换为8的倍数
        /// </summary>
        /// <returns></returns>
        public static int RoundTo(int value, int roundNum)
        {
            return (value + (roundNum-1)) & ~(roundNum-1); 
        }
    }
}

