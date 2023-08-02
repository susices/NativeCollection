// See https://aka.ms/new-console-template for more information

namespace NativeCollection;

public static class Program
{
    public static unsafe void Main()
    {
        var stack = Internal.Stack<int>.Create();
        stack->Push(1);
        stack->Push(2);
        stack->Push(3);
        stack->Push(4);
        stack->Push(5);
        stack->Push(6);
        stack->Push(7);
        stack->Push(8);
        stack->Push(9);
        stack->Push(10);
        stack->Push(11);
        // stack->Push(12);
        while (stack->Count != 0) Console.WriteLine(stack->Pop());

        Console.WriteLine(stack->ArrayLength);


        // Queue<int>* queue = Queue<int>.Create(5);
        // queue->Enqueue(1);
        // queue->Enqueue(2);
        // queue->Enqueue(3);
        // queue->Enqueue(4);
        // queue->Enqueue(5);
        // queue->Enqueue(6);
        // queue->Enqueue(7);
        // queue->Enqueue(1);
        // queue->Enqueue(2);
        // queue->Enqueue(3);
        // queue->Enqueue(4);
        // queue->Enqueue(5);
        // queue->Enqueue(6);
        // queue->Enqueue(7);
        // queue->Enqueue(1);
        // queue->Enqueue(2);
        // queue->Enqueue(3);
        // queue->Enqueue(4);
        // queue->Enqueue(5);
        // queue->Enqueue(6);
        // queue->Enqueue(7);
        //
        // while (queue->Count!=0)
        // {
        //     Console.WriteLine(queue->Dequeue());
        // }

        var sortedSet = new Internal.SortedSet<int>();
        //sortedSet.Add(1);
        sortedSet.Add(2);
        //sortedSet.Add(3);
        sortedSet.Add(4);
        //sortedSet.Add(5);
        sortedSet.Add(6);
        //sortedSet.Add(7);
        sortedSet.Add(8);
        //sortedSet.Add(9);
        sortedSet.Add(10);

        //using SortedSet<int>.Enumerator enumerator = sortedSet.GetEnumerator();

        // foreach (int value in sortedSet)
        // {
        //     Console.WriteLine(value);
        // }

        // sortedSet.Remove(2);
        // sortedSet.Remove(4);
        // sortedSet.Remove(6);
        // sortedSet.Remove(8);
        // sortedSet.Remove(10);
        // foreach (int value in sortedSet)
        // {
        //     Console.WriteLine(value);
        // }
        // sortedSet.Dispose();
    }
}