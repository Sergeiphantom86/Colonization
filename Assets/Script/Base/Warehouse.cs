using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class Warehouse : MonoBehaviour
{
    private Base _botBase;
    private Queue<Resource> _resources;
    public event Action<int> QuantityHasChanged;

    private void Awake()
    {
        _botBase = GetComponent<Base>();
        _resources = new Queue<Resource>();
    }

    private void OnEnable()
    {
        _botBase.CanPay += TrySpendResource;
        _botBase.OnPutItem += AddResource;
    }

    private void OnDisable()
    {
        _botBase.CanPay -= TrySpendResource;
        _botBase.OnPutItem -= AddResource;
    }

    public void AddResource(Resource resource)
    {
        if (resource == null) return;

        StoringResource(resource);

        QuantityHasChanged?.Invoke(_resources.Count);
    }

    private void StoringResource(Resource resource)
    {
        if (resource == null) return;

        _resources.Enqueue(resource);
        resource.transform.SetParent(transform);
        resource.transform.position = transform.position;
    }

    private bool TrySpendResource(int quality)
    {
        if (quality == _resources.Count)
        {
            for (int i = 0; i < quality; i++)
            {
                Destroy(_resources.Dequeue());
            }

            QuantityHasChanged?.Invoke(_resources.Count);

            return true;
        }

        return false;
    }
}