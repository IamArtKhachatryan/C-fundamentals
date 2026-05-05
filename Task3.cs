using System;

public class MyList
{
    private int[] _items;
    private int _count;
    private int _initialCapacity;

    public MyList(int InitialCapacity = 4)
    {
        _initialCapacity = InitialCapacity;
        _items = new int[_initialCapacity];
        _count = 0;
    }

    public int Count => _count;

    public void Add(int item)
    {
        EnsureCapacity(_count + 1);
        _items[_count] = item;
        _count++;
    }
    public void AddRange(int[] items)
    {
        if (items == null) return;

        EnsureCapacity(_count + items.Length);
        foreach (var item in items)
        {
            _items[_count++] = item;
        }
    }

    public bool Remove(int item)
    {
        int index = IndexOf(item);
        if (index == -1) return false;

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _count--;
        _items[_count] = 0;
        return true;
    }

    public bool TryGet(int index, out int value)
    {
        if (index < 0 || index >= _count)
        {
            value = 0;
            return false;
        }

        value = _items[index];
        return true;
    }

    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? _initialCapacity : _items.Length * 2;

            if (newCapacity < min) newCapacity = min;

            int[] newArray = new int[newCapacity];
            Array.Copy(_items, 0, newArray, 0, _count);
            _items = newArray;
        }
    }
    public int this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
            _items[index] = value;
        }
    }

    public int IndexOf(int item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_items[i] == item) return i;
        }
        return -1;
    }

    public bool Contains(int item)
    {
        return IndexOf(item) != -1;
    }

    public void Clear()
    {
        _items = new int[_initialCapacity];
        _count = 0;
    }
}
class Program
{
    static void Main(string[] args)
    {
        MyList list = new MyList(5);

        
        list.Add(17);
        list.Add(12);
        list.Add(34);
        list.Add(40);
        list.Add(55); 

        list.AddRange(new int[] { 47, 84 });

        list.Remove(34);

        if (list.TryGet(2, out int val))
        {
            Console.WriteLine($"Value at index 2 is: {val}");
        }

        list[0] = 100;

        Console.WriteLine($"Count of elements: {list.Count}");

        for (int i = 0; i < list.Count; i++)
        {
            Console.Write(list[i] + " ");
        }
    }
}
