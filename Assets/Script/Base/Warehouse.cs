using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class Warehouse : MonoBehaviour
{
    private Base _botBase;
    private Queue<Resource> _resources;
    public event Action<int> QuantityHasChanged;
    public event Action CanCreate;

    private void Awake()
    {
        _botBase = GetComponent<Base>();
        _resources = new Queue<Resource>();
    }

    private void OnEnable()
    {
        _botBase.OnPutItem += AddResource;
    }

    private void OnDisable()
    {
        _botBase.OnPutItem -= AddResource;
    }

    public void AddResource(Resource resource, int quantityForPayment)
    {
        if (resource == null) return;

        AddStorageResource(resource);
        TrySpendResource(quantityForPayment);

        QuantityHasChanged?.Invoke(_resources.Count);
    }

    private void AddStorageResource(Resource resource)
    {
        if (resource == null) return;

        _resources.Enqueue(resource);
        resource.transform.SetParent(transform);
        resource.transform.position = transform.position;
    }

    private void TrySpendResource(int quantityForPayment)
    {
        if (_resources.Count < quantityForPayment) return;
        
        for (int i = 0; i < quantityForPayment; i++)
        {
            Destroy(_resources.Dequeue());
        }

        CanCreate?.Invoke();
    }
}