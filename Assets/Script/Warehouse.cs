using System;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    private List<Resource> _resources;

    public event Action<int> QuantityHasChanged;

    private void Awake()
    {
        _resources = new List<Resource>();
    }

    public void AddRecource(Resource resource)
    {
        if (resource == null)
            throw new ArgumentOutOfRangeException(nameof(resource));

        _resources.Add(resource);
        
        QuantityHasChanged?.Invoke(_resources.Count);
    }
}