using NativeCollection;

public static class Program
{
    public static void Main()
    {
        MultiMap<int, int> multiMap = new ();
        while (true)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    multiMap.Add(i,j);
                }
                for (int j = 0; j < 10; j++)
                {
                    multiMap.Remove(i,j);
                }
            }
            multiMap.Clear();
        }
    }
}