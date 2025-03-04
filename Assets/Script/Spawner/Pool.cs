using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private Stack<T> _objects = new Stack<T>();

    private Func<T> _createFunc;

    public Pool(Func<T> createFunc)
    {
        _createFunc = () =>
        {
            T obj = createFunc();
            _objects.Push(obj);

            return obj;
        };
    }

    public void Release(T @object) =>
        _objects.Push(@object);

    public T GetObject()
    {
        if (_objects.Count == 0)
            _createFunc();

        return _objects.Pop();
    }
}