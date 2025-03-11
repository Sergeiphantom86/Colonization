using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyResourceIdentifier : MonoBehaviour
{
    private Base _botBase;
    private Queue<Resource> _sortedResources;
    public event Action<Queue<Resource>> SortedResources;

    private void Awake()
    {
        _sortedResources = new();

        _botBase = GetComponentInChildren<Base>();
    }

    private void OnEnable() => _botBase.HasSortedResources += TryGetNearestResource;

    private void OnDisable() => _botBase.HasSortedResources -= TryGetNearestResource;

    private void TryGetNearestResource(List<Resource> detectedResources, Vector3 positionBase)
    {
        _sortedResources = new Queue<Resource>(detectedResources
            .Where(resource => resource != null)
            .OrderBy(resource => Vector3.Distance(resource.transform.position, positionBase)));

        SortedResources?.Invoke(_sortedResources);
    }
}