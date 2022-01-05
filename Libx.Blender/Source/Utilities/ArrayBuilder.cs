namespace Libx.Blender.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable


/*/ Simple list with only insert functions.

    Currently this implementation is a bit unnecessary, standard class list can be used.
    previously I had written this implementation mainly to be able to access the internal array.
    See: ArrayBuilder <T>.ArrayBuilder (ref T[] array)
/*/

public class ArrayBuilder <T> : IEnumerable<T>
{
    private T[] m_array;

    public int Count { get; private set; }

    public ArrayBuilder (int capacity)
    {
        m_array = new T[capacity];
        Count = 0;
    }

    public ArrayBuilder (ref T[] array)
    {
        m_array = array;
        Count = 0;
    }

    public T this[int index] => m_array[index];

    public void Add (T item)
    {
        int tot = Count + 1;
        EnsureCapacity (tot);
        m_array[Count] = item;
        Count = tot;
    }

    public void AddRange (T[] items)
    {
        int count = items.Length;
        int tot = Count + count;
        EnsureCapacity (tot);
        for (int i = 0, j = Count; i < count; i++, j++ )
            m_array[j] = items[i];
        Count = tot;
    }

    public void AddRange (IList <T> items)
    {
        int count = items.Count;
        int tot = Count + count;
        EnsureCapacity (tot);
        for (int i = 0, j = Count; i < count; i++, j++ )
            m_array[j] = items[i];
        Count = tot;
    }

    void EnsureCapacity (int capacity)
    {
        if (m_array.Length < capacity)
        {
            int newSize = Math.Max(m_array.Length * 2, capacity);
            Array.Resize (ref m_array, newSize);
        }
    }

    public IEnumerator<T> GetEnumerator ()
    {
        for (int i = 0; i < Count; i++)
            yield return m_array[i];
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

    public T[] ToArray ()
    {
        var arr = new T[Count];
        Array.Copy (m_array, arr, Count);
        return arr;
    }

    public List<T> ToList ()
    {
        return new List<T> (m_array);
    }
}
