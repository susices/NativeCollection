using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeCollection.Internal;

public unsafe struct List<T>:ICollection<T>, IDisposable where T : unmanaged, IEquatable<T>
{
    private List<T>* self;
    
    private int _size;

    private int _arrayLength;

    private T* _items;
    
    private const int _defaultCapacity = 4;

    public static List<T>* Create(int initialCapacity = _defaultCapacity)
    {
        if (initialCapacity < 0) ThrowHelper.ListInitialCapacityException();

        var list = (List<T>*)NativeMemoryHelper.Alloc((uint)Unsafe.SizeOf<List<T>>());
        GC.AddMemoryPressure(Unsafe.SizeOf<List<T>>());

        if (initialCapacity < _defaultCapacity)
            initialCapacity = _defaultCapacity; // Simplify doubling logic in Push.

        list->_items = (T*)NativeMemoryHelper.Alloc((uint)initialCapacity, (uint)Unsafe.SizeOf<T>());
        GC.AddMemoryPressure(initialCapacity * Unsafe.SizeOf<T>());
        list->_arrayLength = initialCapacity;
        list->_size = 0;
        list->self = list;
        return list;
    }
    
    public int Capacity
    {
        get => _arrayLength;
        set
        {
            if (value < _size)
            {
                ThrowHelper.ListSmallCapacity();
            }

            if (value != _arrayLength)
            {
                if (value > 0)
                {
                    T* newArray = (T*)NativeMemoryHelper.Alloc((UIntPtr)value, (UIntPtr)Unsafe.SizeOf<T>());
                    if (_size > 0)
                    {
                        Unsafe.CopyBlockUnaligned(newArray, _items, (uint)(_arrayLength * Unsafe.SizeOf<T>()));
                    }
                    NativeMemoryHelper.Free(_items);
                    GC.RemoveMemoryPressure(Unsafe.SizeOf<T>()*_arrayLength);
                    _items = newArray;
                    _arrayLength = value;
                }
                else
                {
                    ThrowHelper.ListSmallCapacity();
                }
            }
        }
    }
    
    public void Add(T value)
    {
        T* array = _items;
        int size = _size;
        if ((uint)size < (uint)_arrayLength)
        {
            _size = size + 1;
            array[size] = value;
        }
        else
        {
            AddWithResize(value);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        Console.WriteLine($"index: {index}");
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public int IndexOf(in T item)
    {
        for (int i = 0; i < _size; i++)
        {
            if (_items[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }
    
    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            ThrowHelper.IndexMustBeLessException();
        }
        _size--;
        if (index < _size)
        {
            Unsafe.CopyBlockUnaligned(_items+index, _items+index + 1, (uint)((_size - index)*Unsafe.SizeOf<T>()));
        }
    }
    

    public void Clear()
    {
        _size = 0;
    }

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public int Count => _size;
    public bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.IndexMustBeLessException();
            }
            return _items[index];
        }
    
        set
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.IndexMustBeLessException();
            }
            _items[index] = value;
        }
    }
    
    private void AddWithResize(in T item)
    {
        int size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items[size] = item;
    }
    
    private void Grow(int capacity)
    {
        Debug.Assert(_arrayLength < capacity);

        int newcapacity = _arrayLength == 0 ? _defaultCapacity : 2 * _size;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)newcapacity > Array.MaxLength) newcapacity = Array.MaxLength;

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newcapacity < capacity) newcapacity = capacity;

        Capacity = newcapacity;
    }

    public Span<T> AsSpan()
    {
        return new Span<T>(_items, _size);
    }
    
    public void Dispose()
    {
        NativeMemoryHelper.Free(_items);
        GC.RemoveMemoryPressure(_arrayLength * Unsafe.SizeOf<T>());
    }
    
    
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public Enumerator GetEnumerator()
    {
        return new Enumerator(self);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (T item in *self)
        {
            sb.Append($"{item}  ");
        }
        return sb.ToString();
    }

    public struct Enumerator : IEnumerator<T>
    {
        object IEnumerator.Current => Current;

        private int CurrentIndex;

        private T CurrentItem;

        private List<T>* Items;

        internal Enumerator(List<T>* items)
        {
            Items = items;
            Initialize();
        }

        private void Initialize()
        {
            CurrentIndex = 0;
            CurrentItem = default;
        }
        
        public bool MoveNext()
        {
            if (CurrentIndex==Items->_size)
            {
                return false;
            }

            CurrentItem = Items->_items[CurrentIndex];

            CurrentIndex++;
            return true;
        }

        public void Reset()
        {
            Initialize();
        }

        public T Current
        {
            get
            {
                return CurrentItem;
            }
        }

        public void Dispose()
        {
            
        }
    }
}