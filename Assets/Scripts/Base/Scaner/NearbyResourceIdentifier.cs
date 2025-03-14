using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyResourceIdentifier : MonoBehaviour
{
    private Queue<Resource> _sortedResources;

    private void Awake()
    {
        _sortedResources = new Queue<Resource>();
    }

    public Queue<Resource> TryGetNearestResources(List<Resource> detectedResources, Vector3 positionBase)
    {
        _sortedResources = new Queue<Resource>(detectedResources
            .Where(resource => resource != null)
            .OrderBy(resource => Vector3.Distance(resource.transform.position, positionBase)));

        return _sortedResources;
    }
}