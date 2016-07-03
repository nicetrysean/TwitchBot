﻿using System.Collections;
using System.Collections.Generic;
using System;

public class ShuffleBag<T> : ICollection<T>, IList<T>
{
    private List<T> data = new List<T>();
    private int cursor = 0;
    private T last;

    /// <summary>
    /// Get the next value from the ShuffleBag
    /// </summary>
    public T Next()
    {
        if (cursor < 1)
        {
            cursor = data.Count - 1;
            if (data.Count < 1)
                return default(T);
            return data[0];
        }
        int grab = (int)Math.Floor(new Random().NextDouble() * (cursor + 1));
        T temp = data[grab];
        data[grab] = this.data[this.cursor];
        data[cursor] = temp;
        cursor--;
        return temp;
    }

    //This Constructor will let you do this: ShuffleBag<int> intBag = new ShuffleBag<int>(new int[] {1, 2, 3, 4, 5});
    public ShuffleBag(T[] initalValues)
    {
        for (int i = 0; i < initalValues.Length; i++)
        {
            Add(initalValues[i]);
        }
    }
    public ShuffleBag() { } //Constructor with no values

    #region IList[T] implementation
    public int IndexOf(T item)
    {
        return data.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        cursor = data.Count;
        data.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        cursor = data.Count - 2;
        data.RemoveAt(index);
    }

    public T this[int index]
    {
        get
        {
            return data[index];
        }
        set
        {
            data[index] = value;
        }
    }
    #endregion

    #region IEnumerable[T] implementation
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return data.GetEnumerator();
    }
    #endregion

    #region ICollection[T] implementation
    public void Add(T item)
    {
        //Debug.Log (item);
        data.Add(item);
        cursor = data.Count - 1;
    }

    public int Count
    {
        get
        {
            return data.Count;
        }
    }

    public void Clear()
    {
        data.Clear();
    }

    public bool Contains(T item)
    {
        return data.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in data)
        {
            array.SetValue(item, arrayIndex);
            arrayIndex = arrayIndex + 1;
        }
    }

    public bool Remove(T item)
    {
        cursor = data.Count - 2;
        return data.Remove(item);
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }
    #endregion

    #region IEnumerable implementation
    IEnumerator IEnumerable.GetEnumerator()
    {
        return data.GetEnumerator();
    }
    #endregion

}
