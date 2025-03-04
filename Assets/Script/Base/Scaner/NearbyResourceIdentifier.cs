using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Scanner))]
public class NearbyResourceIdentifier : MonoBehaviour
{
    private Scanner _scanner;
    private List<Resource> _sortedResources;

    private void Awake()
    {
        _sortedResources = new();

        _scanner = GetComponentInChildren<Scanner>();
    }

    private void OnEnable() => _scanner.HasSortedResources += TryGetNearestResource;

    private void OnDisable() => _scanner.HasSortedResources -= TryGetNearestResource;

    public List<Resource> TryGetNearestResource(List<Resource> detectedResources, Vector3 positionBase)
    {
        _sortedResources = detectedResources
            .Where(resource => resource != null)
            .OrderBy(resource => Vector3.Distance(resource.transform.position, positionBase))
            .ToList();
        Debug.Log(_sortedResources.Count);
        return _sortedResources;
    }
}