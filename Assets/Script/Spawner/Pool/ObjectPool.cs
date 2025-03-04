using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> _items = new();
    private readonly Func<T> _createFunc;

    public ObjectPool(Func<T> createFunc)
    {
        _createFunc = createFunc;
    }
    public T Get()
    {
        T item;
        if (_items.Count == 0)
        {
            item = _createFunc();
        }
        else
        {
            item = _items.Dequeue();
        }
       
        return item;
    }

    public void Release(T item)
    {
        _items.Enqueue(item);
    }
}