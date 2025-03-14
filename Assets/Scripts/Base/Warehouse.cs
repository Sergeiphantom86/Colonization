using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class Warehouse : MonoBehaviour
{
    private Base _botBase;
    private Queue<Resource> _resources;

    public int QuantityResources { get { return _resources.Count; } private set { } }

    public event Action<int> QuantityHasChanged;

    private void Awake()
    {
        _botBase = GetComponent<Base>();
        _resources = new Queue<Resource>();
    }

    public void AddResource(Resource resource)
    {
        if (resource == null) return;

        AddStorageResource(resource);

        ChangeQuantityResources();
    }

    public void RemoveResource(int quantityForPayment)
    {
        for (int i = 0; i < quantityForPayment; i++)
        {
            Destroy(_resources.Dequeue());
        }

        ChangeQuantityResources();
    }

    private void AddStorageResource(Resource resource)
    {
        if (resource == null) return;

        _resources.Enqueue(resource);
        resource.transform.SetParent(transform);
        resource.transform.position = transform.position;
    }

    private void ChangeQuantityResources()
    {
        QuantityHasChanged?.Invoke(_resources.Count);
    }
}